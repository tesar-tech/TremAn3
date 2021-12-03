using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.Services;
using Windows.Media.Protection.PlayReady;
using Windows.UI;
using Windows.UI.Xaml;

namespace TremAn3.ViewModels
{
    public class FreqCounterViewModel : ViewModelBase
    {
        public FreqCounterViewModel(PlotModelsContainerViewModel pmcvm)
        {
            PlotModelsContainerViewModel = pmcvm;
        }


        public MainViewModel ParentVm { get => ViewModelLocator.Current.MainViewModel; }

        public PlotModelsContainerViewModel PlotModelsContainerViewModel { get; private set; }

        public void ResetFreqCounter()
        {
            ResetResultDisplay();
            DrawingRectanglesViewModel.RemoveRois();
            Maximum = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds;
            DrawingRectanglesViewModel.MaxHeight = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Height;
            DrawingRectanglesViewModel.MaxWidth = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Width;
            DrawingRectanglesViewModel.plotsNeedRefresh += RefreshPlots;
            MediaControllingViewModel.PositionChanged += MediaControllingViewModel_PositionChanged;
        }
        //timer for invalidation plot
        private DispatcherTimer _annotationTimer;

        private void StartTimer()
        {
            if (_annotationTimer is null)
            {
                _annotationTimer = new DispatcherTimer();
                _annotationTimer.Interval = TimeSpan.FromSeconds(0.01);
                _annotationTimer.Tick += _timer_Tick;
            }
            _annotationTimer.Start();
        }



        private void _timer_Tick(object sender, object e)
        {
            //invalidate plots here, so it doesn't slow the slider
            PlotModelsContainerViewModel.InvalidateTimePlots(false);

            _annotationTimer.Stop();
        }

        private void MediaControllingViewModel_PositionChanged(double value)
        {
            timeAnotations.ForEach(x => x.X = value);
            StartTimer();//invalidated on tick
        }


        public DrawingRectanglesViewModel DrawingRectanglesViewModel
        {
            get { return ViewModelLocator.Current.DrawingRectanglesViewModel; }
        }


        private double _ProgressPercentage;

        public double ProgressPercentage
        {
            get => _ProgressPercentage;
            set => Set(ref _ProgressPercentage, value);
        }


        private void RefreshPlots()
        {
            PlotModelsContainerViewModel.InvalidateAllPlots(true);
        }

        public FreqProgressViewModel FreqProgressViewModel { get; set; } = new FreqProgressViewModel();

        public ResultsViewModel CurrentGlobalScopedResultsViewModel { get; set; } = new ResultsViewModel();


        public async Task DisplayPlots(bool doComputation, DataSeriesType? dataSeriesType = null)
        {

            var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();

            //this basically compute the psd, amps spec, etc...
            IEnumerable<Task> tasks = null;
            if (dataSeriesType == null) //do it for all
                tasks = comps.Select(x => x.PrepareForDisplay(FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize, doComputation));
            else
                tasks = comps.Select(x => x.PrepareForDisplay((DataSeriesType)dataSeriesType, FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize));
            if(doComputation)
                ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.ComputingVectorData;
            await Task.WhenAll(tasks);
            timeAnotations.Clear();//discard current anotations (new will be added)
            var plotModels = dataSeriesType == null ? PlotModelsContainerViewModel.PlotModels :
                     PlotModelsContainerViewModel.PlotModels.Where(x => x.DataSeriesType == (DataSeriesType)dataSeriesType);

            foreach (var pmwt in plotModels)
            {

                //continue;//freq progress has its own plotting logic. see regrawfreqProgress

                if (!comps.Any()) return;
                var firstDatares = comps.First().Algorithm.Results.DataResultsDict[pmwt.DataSeriesType];
                if (firstDatares.IsOk)
                {
                    pmwt.PlotModel = new PlotModel();
                    foreach (var comp in comps)
                    {
                        var series = comp.LineSeriesDict[pmwt.DataSeriesType];
                        pmwt.PlotModel.Series.Add(series);//add series with same type (psd, etc..)
                    }

                    if (pmwt.DataSeriesType == DataSeriesType.FreqProgress)
                    {
                        var maxYOfFreqProgress = comps.Max(comp => comp.Algorithm.Results.DataResultsDict[pmwt.DataSeriesType].Y.Max());//set some offset to max
                        pmwt.PlotModel.Axes.Add(new LinearAxis() { Maximum = maxYOfFreqProgress * 1.1, Minimum = 0, MajorTickSize = 2, MinorTickSize = 0.5, Position = AxisPosition.Left, Key = "Vertical" });
                        FreqProgressViewModel.StatusMessage = $"Frequency resolution: {comps.First().Algorithm.frameRate / FreqProgressViewModel.SegmnetSize:F2} Hz. Number of segments computed: {firstDatares.X.Count} ";
                    }
                }
                else
                    pmwt.PlotModel = new PlotModel { Title = firstDatares.ErrorMessage };
                //for com x and com y add anotation
                if (pmwt.DataSeriesType == DataSeriesType.ComX || pmwt.DataSeriesType == DataSeriesType.ComY)
                {
                    var newAno = RecreateAnnotation();
                    timeAnotations.Add(newAno);
                    pmwt.PlotModel.Annotations.Add(newAno);
                }
            }



            if (dataSeriesType == null)//we currently use dataseries type only for freq progress In case of using single dataseries type for scoped
                                       //it has to be changed..
            {
                //global scoped results
                if (doComputation)
                {
                    ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.ComputingGlobalVectorData;
                    await Task.Delay(3000);

                    await CurrentGlobalScopedResultsViewModel.ComputeAllResults(ParentVm.MediaPlayerViewModel.VideoPropsViewModel.FrameRate,
                        comps.Select(x => x.Algorithm.Results.DataResultsDict[DataSeriesType.ComX].Y).ToList(),
                        comps.Select(x => x.Algorithm.Results.DataResultsDict[DataSeriesType.ComY].Y).ToList());
                }

                foreach (var pwmt in PlotModelsContainerViewModel.PlotModelsGlobalScope)
                {
                    var res = CurrentGlobalScopedResultsViewModel.DataResultsDict[pwmt.DataSeriesType];
                    if (res.IsOk)
                    {
                        pwmt.PlotModel = new PlotModel();
                        LineSeries s = new LineSeries();
                        s.ItemsSource = (res.X.Zip(res.Y, (x, y) => new DataPoint(x, y)));
                        pwmt.PlotModel.Series.Add(s);
                        s.Color = OxyColors.Black;
                    }
                    else
                    {
                        pwmt.PlotModel = new PlotModel { Title = res.ErrorMessage };
                    }
                }

            }

            RaisePropertyChanged(nameof(PlotModelsContainerViewModel));
            IsAllResultsNotObsolete = true;
            ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.Off;

        }

        /// <summary>
        /// Freq progress has its own logic how to rewrite plots. It is mainly bcs of different segements sizes for fft. Some of
        /// them does not produce result. And this method is called when segment size is changed
        /// solo caller -> if only freq progress redraw is call, raise prop change, otherwise do it somewhere else
        /// </summary>
        //public void ReDrawFreqProgress(bool isSoloCaller = false)
        //{
        //    var freqProgressPlotModel = new PlotModel();
        //    var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();
        //    if (comps.Count < 1)
        //        return;
        //    //double maxYOfFreqProgress = 0;
        //    foreach (var comp in comps)
        //    {
        //        try
        //        {
        //            //comp.PrepareForDisplayFreqProgress(FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize);
        //        }
        //        catch (FftSettingsException e)
        //        {//when there is an error (i.e. window is longer than signal)
        //            FreqProgressViewModel.StatusMessage = e.Message;
        //            FreqProgressViewModel.IsFreqProgressParametersOk = false;
        //            PlotModelsContainerViewModel.SetFreqProgressToNoData();//display No data (also exception and err message is handled in prepare)
        //            if (isSoloCaller)
        //                RaisePropertyChanged(nameof(PlotModelsContainerViewModel));
        //            return;
        //        }

        //        freqProgressPlotModel.Series.Add(comp.LineSeriesDict[DataSeriesType.FreqProgress]);
        //        //get maximum to better view 
        //        //maxYOfFreqProgress = maxYOfFreqProgress < comp.Algorithm.Results.FreqProgress.Max() ? comp.Algorithm.Results.FreqProgress.Max() : maxYOfFreqProgress;
        //    }


        //    FreqProgressViewModel.IsFreqProgressParametersOk = true;
        //    var freqProgressAnnotation = RecreateAnnotation();
        //    timeAnotations.Add(freqProgressAnnotation);
        //    freqProgressPlotModel.Annotations.Add(freqProgressAnnotation);
        //    //10 is just subtitution for max
        //    var maxYOfFreqProgress = 10;// comps.Max(comp => comp.Algorithm.Results.FreqProgress.Max());

        //    freqProgressPlotModel.Axes.Add(new LinearAxis() { Maximum = maxYOfFreqProgress * 1.1, Minimum = 0, MajorTickSize = 2, MinorTickSize = 0.5, Position = AxisPosition.Left, Key = "Vertical" });
        //    PlotModelsContainerViewModel.PlotModels.First(x => x.DataSeriesType == DataSeriesType.FreqProgress).PlotModel = freqProgressPlotModel;
        //    //PlotModelsContainerViewModel.PlotModels.First(x => x.DataSeriesType == DataSeriesType.FreqProgress).PlotModel.InvalidatePlot(true);
        //    if (isSoloCaller)
        //        RaisePropertyChanged(nameof(PlotModelsContainerViewModel));

        //}


        List<LineAnnotation> timeAnotations = new List<LineAnnotation>();

        LineAnnotation RecreateAnnotation() => new LineAnnotation() { Type = LineAnnotationType.Vertical, ClipByXAxis = false, X = 0, Color = OxyColors.Black };

        double _maximum;

        public double Maximum
        {
            get => _maximum;
            set
            {
                if (_maximum == value) return;
                _maximum = value;
                RaisePropertyChanged();
                Maxrange = Maximum;//on the begining - use full range

            }
        }

        /// <summary>
        /// set plots to NO data
        /// </summary>
        internal void ResetResultDisplay()
        {
            //VideoMainFreq = -1;//means nothing
            PlotModelsContainerViewModel.SetAllModelsToNoData();
            RaisePropertyChanged(nameof(PlotModelsContainerViewModel));
        }

        double _minrange;

        public double Minrange
        {
            get => _minrange;
            set
            {

                if (_minrange == value) return;
                if (Maxrange - value >= 1)
                    _minrange = value;
                isRangeInSettingProcess = true;
                RaisePropertyChanged();
                isRangeInSettingProcess = false;
                MakesResultsObsolete();

            }
        }
        //need this to not "drag" slider value when range changes
        bool isRangeInSettingProcess;

        double _maxrange;

        public double Maxrange
        {
            get => _maxrange;
            set
            {

                if (_maxrange == value) return;
                if (value - Minrange >= 1)
                    _maxrange = value;
                isRangeInSettingProcess = true;
                RaisePropertyChanged();
                isRangeInSettingProcess = false;
                MakesResultsObsolete();
            }
        }

        internal void IsRoiSameAsResultSomeChange(bool iscallingResultNotObsolete)
        {
            if (!iscallingResultNotObsolete)
            {
                IsAllResultsNotObsolete = false;
                if ((0 == DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList().Where(x => x.IsRoiSameAsResult == true).Count()))
                {//no results that match data
                    ResetResultDisplay();
                }
            }
            else
              if (0 == DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList().Where(x => x.IsRoiSameAsResult == false).Count())
                IsAllResultsNotObsolete = true;//all results are same as roi

        }

        private void MakesResultsObsolete()
        {
            DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList().ForEach(async x => await x.IsRoiSameAsResultSet(false));
            //plots are invalidated multiple times, but.. yeah.who cares..
        }

        private bool _IsAllResultsNotObsolete;

        public bool IsAllResultsNotObsolete
        {
            get => _IsAllResultsNotObsolete;
            set => Set(ref _IsAllResultsNotObsolete, value);
        }


        private bool _IsComputationInProgress;

        public bool IsComputationInProgress
        {
            get => _IsComputationInProgress;
            set
            {
                if (Set(ref _IsComputationInProgress, value))
                    if (value)
                        ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.ComputationInProgress;
                    else
                        ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.Off;

            }
        }

        private double _percentageOfResolution = 100;//aka SizeReductionFactor

        public double PercentageOfResolution
        {
            get => _percentageOfResolution;
            set => Set(ref _percentageOfResolution, value);
        }

        private double _SliderPlotValue;

        public double SliderPlotValue
        {
            get => _SliderPlotValue;
            set
            {


                if (Set(ref _SliderPlotValue, value) && !MediaControllingViewModel.IsPositionChangeFromMethod && !isRangeInSettingProcess)
                    MediaControllingViewModel.PositionChangeRequest(value);
            }
        }
        private MediaControllingViewModel MediaControllingViewModel { get => ViewModelLocator.Current.MediaControllingViewModel; }


        public async Task DisplaySpectralAnalysisInfo()
        {

            await DialogService.DisplaySpectralAnalysisInfo();

        }
    }
}
