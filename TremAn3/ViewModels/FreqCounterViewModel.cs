using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using Windows.Media.Protection.PlayReady;
using Windows.UI;
using Windows.UI.Xaml;

namespace TremAn3.ViewModels
{
    public class FreqCounterViewModel : ViewModelBase
    {
        public FreqCounterViewModel()
        {
            _PSDPlotModel = getPlotModelWithNoDataText();
            _XCoMPlotModel = getPlotModelWithNoDataText();
            _YCoMPlotModel = getPlotModelWithNoDataText();
        }
        public MainViewModel ParentVm { get => ViewModelLocator.Current.MainViewModel; }

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
            _annotationTimer?.Start();
        }



        private void _timer_Tick(object sender, object e)
        {
            //invalidate plots here, so it doesn't slow the slider
            XCoMPlotModel.InvalidatePlot(false);
            YCoMPlotModel.InvalidatePlot(false);
            FreqProgressPlotModel.InvalidatePlot(false);
            _annotationTimer.Stop();
        }

        private void MediaControllingViewModel_PositionChanged(double value)
        {
            if (xcomAnnotation == null || ycomAnnotation == null|| freqProgressAnnotation == null) return;
            xcomAnnotation.X = value;
            ycomAnnotation.X = value;
            freqProgressAnnotation.X = value;
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

        private PlotModel _PSDPlotModel;

        public PlotModel PSDPlotModel
        {
            get => _PSDPlotModel;
            set => Set(ref _PSDPlotModel, value);
        }

        private PlotModel _XCoMPlotModel;

        public PlotModel XCoMPlotModel
        {
            get => _XCoMPlotModel;
            set => Set(ref _XCoMPlotModel, value);
        }

        private PlotModel _YCoMPlotModel;

        public PlotModel YCoMPlotModel
        {
            get => _YCoMPlotModel;
            set => Set(ref _YCoMPlotModel, value);
        }

        private PlotModel _FreqProgressPlotModel;

        public PlotModel FreqProgressPlotModel
        {
            get => _FreqProgressPlotModel;
            set => Set(ref _FreqProgressPlotModel, value);
        }


        private void RefreshPlots()
        {
            XCoMPlotModel.InvalidatePlot(true);
            YCoMPlotModel.InvalidatePlot(true);
            PSDPlotModel.InvalidatePlot(true);
            FreqProgressPlotModel.InvalidatePlot(true);
        }

        private PlotModel getPlotModelWithNoDataText()
        {
            var model = new PlotModel { Title = "No Data" };
            return model;
        }

        //public enum PlotType
        //{
        //    XCoM, YCoM, PSDAvg
        //}

        public void DisplayPlots()
        {
            var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();

            var psdPlotModel = new PlotModel();
            var xcomPlotModel = new PlotModel();
            var ycomPlotModel = new PlotModel();
            var freqProgressPlotModel = new PlotModel();
            foreach (var comp in comps)
            {
                comp.PrepareForDisplay();
                psdPlotModel.Series.Add(comp.PsdSeries);
                xcomPlotModel.Series.Add(comp.XComSeries);
                ycomPlotModel.Series.Add(comp.YComSeries);
                freqProgressPlotModel.Series.Add(comp.FreqProgressSeries);
            }
            recreateAnnotations();
            xcomPlotModel.Annotations.Add(xcomAnnotation);
            ycomPlotModel.Annotations.Add(ycomAnnotation);
            freqProgressPlotModel.Annotations.Add(freqProgressAnnotation);

            PSDPlotModel = psdPlotModel;
            XCoMPlotModel = xcomPlotModel;
            YCoMPlotModel = ycomPlotModel;
            FreqProgressPlotModel = freqProgressPlotModel;
            IsAllResultsNotObsolete = true;
        }

        LineAnnotation xcomAnnotation;
        LineAnnotation ycomAnnotation;
        LineAnnotation freqProgressAnnotation;
        void recreateAnnotations()
        {
            xcomAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                ClipByXAxis = false,
                Color = OxyColors.Black,
                X = 0
            };
            ycomAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                ClipByXAxis = false,
                X = 0,
                Color = OxyColors.Black,
            };
            freqProgressAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                ClipByXAxis = false,
                X = 0,
                Color = OxyColors.Black,
            };
        }

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

        internal void ResetResultDisplay()
        {
            //VideoMainFreq = -1;//means nothing
            XCoMPlotModel = getPlotModelWithNoDataText();
            YCoMPlotModel = getPlotModelWithNoDataText();
            PSDPlotModel = getPlotModelWithNoDataText();
            FreqProgressPlotModel = getPlotModelWithNoDataText();

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
            DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList().ForEach(x => x.IsRoiSameAsResult = false);
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
            set => Set(ref _IsComputationInProgress, value);
        }

        private double _percentageOfResolution = 100;//aka SizeReductionFactor

        public double PercentageOfResolution
        {
            get => _percentageOfResolution;
            set => Set(ref _percentageOfResolution, value);
        }

        private double _VideoMainFreq = -1;

        //lower than zero means: no value
        //(same as null, but null does not update raise prop)
        //public double VideoMainFreq
        //{
        //    get => _VideoMainFreq;
        //    set
        //    {
        //        if (_VideoMainFreq == value) return;
        //        _VideoMainFreq = value;
        //        RaisePropertyChanged();
        //    }
        //}


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





    }
}
