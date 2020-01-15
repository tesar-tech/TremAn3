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

        //private PlotModel _PlotModelFreqInTime = new PlotModel();

        //public PlotModel PlotModelFreqInTime
        //{
        //    get => _PlotModelFreqInTime;
        //    set => Set(ref _PlotModelFreqInTime, value);
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
        internal void UpdatePlotsWithNewVals(PlotType type, List<(double xx, double yy)> newVals, bool justClear = false)
        {
            switch (type)
            {
                case PlotType.XCoM:
                    if (justClear || newVals?.Count() <= 0)
                        XCoMPlotModel = getPlotModelWithNoDataText();
                    else
                    {
                        XCoMPlotModel = NewPlotModelWithSeries(newVals);
                        XCoMPlotModel.InvalidatePlot(true);
                        //newPlotModel.Axes[0].Title = "Freq (Hz)";
                        //newPlotModel.Axes[1].Title = "PSD";
                    }
                    break;
                case PlotType.YCoM:
                    if (justClear || newVals?.Count() <= 0)
                        YCoMPlotModel = getPlotModelWithNoDataText();
                    else
                    {
                        YCoMPlotModel = NewPlotModelWithSeries(newVals);
                        YCoMPlotModel.InvalidatePlot(true);
                        //newPlotModel.Axes[0].Title = "Freq (Hz)";
                        //newPlotModel.Axes[1].Title = "PSD";
                    }
                    break;
                case PlotType.PSDAvg:
                    if (justClear || newVals?.Count() <= 0)
                        PSDPlotModel = getPlotModelWithNoDataText();
                    else
                    {
                        PSDPlotModel = NewPlotModelWithSeries(newVals);
                        PSDPlotModel.InvalidatePlot(true);
                        //newPlotModel.Axes[0].Title = "Freq (Hz)";
                        //newPlotModel.Axes[1].Title = "PSD";
                    }
                    break;
                default:
                    break;
            }

        }

        PlotModel NewPlotModelWithSeries(List<(double xx, double yy)> newVals)
        {
            var newPlotModel = new PlotModel();
            LineSeries s = new LineSeries
            {
                ItemsSource = newVals.Select(c => new DataPoint(c.xx, c.yy))
            };
            newPlotModel.Series.Add(s);
            return newPlotModel;
        }

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
            UpdatePlotsWithNewVals(PlotType.XCoM, null, true);
            UpdatePlotsWithNewVals(PlotType.YCoM, null, true);
            UpdatePlotsWithNewVals(PlotType.PSDAvg, null, true);
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
