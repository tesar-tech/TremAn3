﻿using GalaSoft.MvvmLight;
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
            //_PSDPlotModel = getPlotModelWithNoDataText();
            //_XCoMPlotModel = getPlotModelWithNoDataText();
            //_YCoMPlotModel = getPlotModelWithNoDataText();
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
            _annotationTimer?.Start();
        }



        private void _timer_Tick(object sender, object e)
        {
            //invalidate plots here, so it doesn't slow the slider
            PlotModelsContainerViewModel.InvalidateTimePlots(false);

            _annotationTimer.Stop();
        }

        private void MediaControllingViewModel_PositionChanged(double value)
        {
            if (xcomAnnotation == null || ycomAnnotation == null || freqProgressAnnotation == null) return;
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




        private void RefreshPlots()
        {
            PlotModelsContainerViewModel.InvalidateAllPlots(true);
        }

        //private void RefreshFreqProgressPlot() => FreqProgressPlotModel.InvalidatePlot(true);



        public FreqProgressViewModel FreqProgressViewModel { get; set; } = new FreqProgressViewModel();

        //public enum PlotType
        //{
        //    XCoM, YCoM, PSDAvg
        //}

        public void DisplayPlots()
        {
            var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();

            var psdPlotModel = new PlotModel();
            //var ampSpecPlotModel = new PlotModel();
            var xcomPlotModel = new PlotModel();
            var ycomPlotModel = new PlotModel();
            foreach (var comp in comps)
            {
                comp.PrepareForDisplay(FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize);
                psdPlotModel.Series.Add(comp.LineSeriesDict[DataSeriesType.Psd]);
                xcomPlotModel.Series.Add(comp.LineSeriesDict[DataSeriesType.ComX]);
                ycomPlotModel.Series.Add(comp.LineSeriesDict[DataSeriesType.ComY]);

            }
            xcomAnnotation =
            ycomAnnotation = RecreateAnnotation();
            xcomPlotModel.Annotations.Add(xcomAnnotation);
            ycomPlotModel.Annotations.Add(ycomAnnotation);

            PlotModelsContainerViewModel.PSDPlotModel = psdPlotModel;
            PlotModelsContainerViewModel.XCoMPlotModel = xcomPlotModel;
            PlotModelsContainerViewModel.YCoMPlotModel = ycomPlotModel;

            //this basically compute the psd, amps spec, etc...
            comps.ForEach(comp => comp.PrepareForDisplay(FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize));

            foreach (var pmwt in PlotModelsContainerViewModel.PlotModels)
            {
                pmwt.PlotModel = new PlotModel();
                foreach (var comp in comps)
                {
                    pmwt.PlotModel.Series.Add(comp.LineSeriesDict[pmwt.DataSeriesType]);
                }
                if(pmwt.DataSeriesType == DataSeriesType.ComX || pmwt.DataSeriesType == DataSeriesType.ComY)
                    pmwt.PlotModel.Annotations.Add(RecreateAnnotation());


            }
            PlotModelsContainerViewModel.RaisePlotModelsPropChange();


            ReDrawFreqProgress();
            IsAllResultsNotObsolete = true;
        }

        public void ReDrawFreqProgress()
        {
            var freqProgressPlotModel = new PlotModel();
            var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();
            if (comps.Count < 1)
                return;
            double maxYOfFreqProgress = 0;
            foreach (var comp in comps)
            {
                try
                {
                    comp.PrepareForDisplayFreqProgress(FreqProgressViewModel.Step, FreqProgressViewModel.SegmnetSize);
                }
                catch (FftSettingsException e)
                {//when there is an error (i.e. window is longer than signal)
                    FreqProgressViewModel.StatusMessage = e.Message;
                    FreqProgressViewModel.IsFreqProgressParametersOk = false;
                    PlotModelsContainerViewModel.SetFreqProgressToNoData();//display No data (also exception and err message is handled in prepare)
                    return;
                }
              

                freqProgressPlotModel.Series.Add(comp.LineSeriesDict[DataSeriesType.FreqProgress]);
                //get maximum to better view 
                maxYOfFreqProgress = maxYOfFreqProgress < comp.Algorithm.Results.FreqProgress.Max() ? comp.Algorithm.Results.FreqProgress.Max() : maxYOfFreqProgress;
            }

            FreqProgressViewModel.StatusMessage = $"Frequency resolution: {comps.First().Algorithm.frameRate / FreqProgressViewModel.SegmnetSize:F2} Hz. Number of segments computed: {comps.First().Algorithm.Results.FreqProgress.Count} ";

            FreqProgressViewModel.IsFreqProgressParametersOk = true;
            freqProgressAnnotation = RecreateAnnotation();
            freqProgressPlotModel.Annotations.Add(freqProgressAnnotation);
            freqProgressPlotModel.Axes.Add(new LinearAxis() { Maximum = maxYOfFreqProgress * 1.1, Minimum = 0, MajorTickSize = 2, MinorTickSize = 0.5, Position = AxisPosition.Left, Key = "Vertical" });
            PlotModelsContainerViewModel.FreqProgressPlotModel = freqProgressPlotModel;

            PlotModelsContainerViewModel.FreqProgressPlotModel.InvalidatePlot(true);
        }

        LineAnnotation xcomAnnotation;
        LineAnnotation ycomAnnotation;
        LineAnnotation freqProgressAnnotation;

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
