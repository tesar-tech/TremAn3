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
using Windows.UI;

namespace TremAn3.ViewModels
{
    public class FreqCounterViewModel : ViewModelBase
    {
        public FreqCounterViewModel(MainViewModel mainViewModel)
        {
            _PSDPlotModel = getPlotModelWithNoDataText();
            _XCoMPlotModel = getPlotModelWithNoDataText();
            _YCoMPlotModel = getPlotModelWithNoDataText();
            ParentVm = mainViewModel;
            //_PlotModelFreqInTime = getPlotModelWithNoDataText();
        }

        public MainViewModel ParentVm { get;private set; }

        public void ResetFreqCounter()
        {
            ResetResultDisplay();
            DrawingRectanglesViewModel.RemoveRois();
            //RemoveSelection();
            Maximum = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds;
            DrawingRectanglesViewModel.MaxHeight = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Height;
            DrawingRectanglesViewModel.MaxWidth = ParentVm.MediaPlayerViewModel.VideoPropsViewModel.Width;

        }

        //private PlotModel _PlotModelFreqInTime = new PlotModel();

        //public PlotModel PlotModelFreqInTime
        //{
        //    get => _PlotModelFreqInTime;
        //    set => Set(ref _PlotModelFreqInTime, value);
        //}
        //public void RemoveSelection()
        //{
        //    //Rect = (0, 0, 0, 0);
        //    //RemoveSelectionHandler?.Invoke();
        //    SelectionRectangleViewModel.IsVisible = false;
        //}

        //public event Action RemoveSelectionHandler;


        //private SelectionRectangleViewModel _SelectionRectangleViewModel = new SelectionRectangleViewModel();

        //public SelectionRectangleViewModel SelectionRectangleViewModel
        //{
        //    get => _SelectionRectangleViewModel;
        //    set => Set(ref _SelectionRectangleViewModel, value);
        //}

        private DrawingRectanglesViewModel DrawingRectanglesViewModel
        {
            get { return ViewModelLocator.Current.DrawingRectanglesViewModel; }
        }

        //private (uint X, uint Y, uint width, uint height) _Rect;

        //public (uint X, uint Y, uint width, uint height) Rect
        //{
        //    get => _Rect;
        //    set
        //    {
        //        Set(ref _Rect, value);
        //    }
        //}


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


        private PlotModel getPlotModelWithNoDataText()
        {
            var model = new PlotModel { Title = "No Data" };
            return model;
        }

        public enum PlotType
        {
            XCoM,YCoM,PSDAvg
        }

        public void DisplayPlots()
        {
            var comps = DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel).ToList();
            
            var psdPlotModel = new PlotModel();
            var xcomPlotModel = new PlotModel();
            foreach (var comp in comps)
            {
                comp.PrepareForDisplay();
                psdPlotModel.Series.Add(comp.PsdSeries);
                xcomPlotModel.Series.Add(comp.XComSeries);
            }

            PSDPlotModel = psdPlotModel;
            XCoMPlotModel = xcomPlotModel;


        }
        //internal void PlotComAlgs(List<CenterOfMotionAlgorithm> comAlgs)
        //{

        //    foreach (var comAlg in comAlgs)
        //    {
        //        comAlg.GetMainFreqAndFillPsdDataFromComLists();

        //    }
        //    var PSDs = comAlgs.Select(x => x.PsdAvgData);
        //    UpdatePlotsWithNewVals(PlotType.PSDAvg, comAlgs);


        //    //psd

        //    //x
        //    var xComs = comAlgs[0].ListComXNoAvg.Select((x, i) => ((double)i, x)).ToList();
        //    FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.XCoM, xComs);

        //    //y
        //    var yComs = comAlgs[0].ListComYNoAvg.Select((x, i) => ((double)i, x)).ToList();
        //    FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.YCoM, yComs);

        //}

        //internal void UpdatePlotsWithNewVals(PlotType type, List<List<(double xx, double yy)>> newVals, bool justClear = false)
        //{
        //    switch (type)
        //    {
        //        case PlotType.XCoM:
        //            if (justClear || newVals?.Count() <= 0)
        //                XCoMPlotModel = getPlotModelWithNoDataText();
        //            else
        //            {
        //                XCoMPlotModel = NewPlotModelWithSeries(newVals);
        //                XCoMPlotModel.InvalidatePlot(true);
        //                //newPlotModel.Axes[0].Title = "Freq (Hz)";
        //                //newPlotModel.Axes[1].Title = "PSD";
        //            }
        //            break;
        //        case PlotType.YCoM:
        //            if (justClear || newVals?.Count() <= 0)
        //            else
        //            {
        //                YCoMPlotModel = NewPlotModelWithSeries(newVals);
        //                YCoMPlotModel.InvalidatePlot(true);
        //                //newPlotModel.Axes[0].Title = "Freq (Hz)";
        //                //newPlotModel.Axes[1].Title = "PSD";
        //            }
        //            break;
        //        case PlotType.PSDAvg:
        //            if (justClear || newVals?.Count() <= 0)
        //            else
        //            {
        //                PSDPlotModel = NewPlotModelWithSeries(newVals);
        //                PSDPlotModel.InvalidatePlot(true);
        //                //newPlotModel.Axes[0].Title = "Freq (Hz)";
        //                //newPlotModel.Axes[1].Title = "PSD";
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //}

        //PlotModel NewPlotModelWithSeries(List<List<(double xx, double yy)>> series)
        //{
        //    var newPlotModel = new PlotModel();
        //    //foreach (var vals in series)
        //    //{
        //    //    LineSeries s = new LineSeries
        //    //    {
        //    //        ItemsSource = vals.Select(c => new DataPoint(c.xx, c.yy)),
        //    //        Color = 
        //    //    };
        //    //}
  
        //    //Random dn = new Random();
        //    //LineSeries s2 = new LineSeries
        //    //{
        //    //    ItemsSource = series.Select(c => new DataPoint(c.xx, c.yy + dn.NextDouble())),
        //    //    Color = OxyColor.FromArgb(255, 255, 0, 255)
        //    //};
        //    //newPlotModel.Series.Add(s);
        //    //newPlotModel.Series.Add(s2);
        //    return newPlotModel;
        //}

        //private bool _IsDataAvailableForPlot;//for displaying no data over plot

        //public bool IsDataAvailableForPlot
        //{
        //    get => _IsDataAvailableForPlot;
        //    set => Set(ref _IsDataAvailableForPlot, value);
        //}



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
            VideoMainFreq = -1;//means nothing
            XCoMPlotModel = getPlotModelWithNoDataText();
            YCoMPlotModel = getPlotModelWithNoDataText();
            PSDPlotModel = getPlotModelWithNoDataText();

        }

        double _minrange;

        public double Minrange
        {
            get => _minrange;
            set
            {

                if (_minrange == value) return;
                if (Maxrange - value < 1)
                {
                    RaisePropertyChanged();
                    return;
                }
                _minrange = value;
                RaisePropertyChanged();

            }
        }

        double _maxrange;

        public double Maxrange
        {
            get => _maxrange;
            set
            {

                if (_maxrange == value) return;
                if (value - Minrange < 1)
                {
                    RaisePropertyChanged();
                    return;
                }
                _maxrange = value;
                RaisePropertyChanged();

            }
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
        public double VideoMainFreq
        {
            get => _VideoMainFreq;
            set
            {
                if (_VideoMainFreq == value) return;
                _VideoMainFreq = value;
                RaisePropertyChanged();
            }
        }




    }
}
