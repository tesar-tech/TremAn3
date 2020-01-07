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
        public FreqCounterViewModel()
        {
            _PSDPlotModel = getPlotModelWithNoDataText();
            _PlotModelFreqInTime = getPlotModelWithNoDataText();
        }

        private PlotModel _PlotModelFreqInTime = new PlotModel();

        public PlotModel PlotModelFreqInTime
        {
            get => _PlotModelFreqInTime;
            set => Set(ref _PlotModelFreqInTime, value);
        }

        private PlotModel _PSDPlotModel;

        public PlotModel PSDPlotModel
        {
            get => _PSDPlotModel;
            set => Set(ref _PSDPlotModel, value);
        }

        private PlotModel getPlotModelWithNoDataText()
        {
            var model = new PlotModel { Title = "No Data" };
            return model;
        }

        internal void UpdatePlotsWithNewVals(IEnumerable<(double xx, double yy)> newValsFreqInTime, List<(double xx, double yy)> newValsPSD, bool justClear = false)
        {

            if (justClear || newValsFreqInTime?.Count() <= 0)
                PlotModelFreqInTime = getPlotModelWithNoDataText();
            else
            {
                var newPlotModel = new PlotModel();
                LineSeries s = new LineSeries
                {
                    ItemsSource = newValsFreqInTime.Select(c => new DataPoint(c.xx, c.yy))
                };
                newPlotModel.Series.Add(s);




                PlotModelFreqInTime = newPlotModel;
                PlotModelFreqInTime.InvalidatePlot(true);
                newPlotModel.Axes[0].Title = "Time (s)";
                newPlotModel.Axes[1].Title = "Freq (Hz)";

                


            }
            if (justClear || newValsPSD?.Count() <= 0)
                PSDPlotModel = getPlotModelWithNoDataText();
            else
            {
                var newPlotModel = new PlotModel();
                LineSeries s = new LineSeries
                {
                    ItemsSource = newValsPSD.Select(c => new DataPoint(c.xx, c.yy))
                };
                newPlotModel.Series.Add(s);
                PSDPlotModel = newPlotModel;
                PSDPlotModel.InvalidatePlot(true);
                newPlotModel.Axes[0].Title = "Freq (Hz)";
                newPlotModel.Axes[1].Title = "PSD";
            }
            
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
            UpdatePlotsWithNewVals(null, null, true);
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
