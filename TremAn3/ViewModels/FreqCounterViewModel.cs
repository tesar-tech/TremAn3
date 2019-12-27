using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
    public class FreqCounterViewModel : ViewModelBase
    {
        public FreqCounterViewModel()
        {
            LineSeries ls = new LineSeries();
            ls.ItemsSource = DataPoints;
            PlotModel.Series.Add(ls);
            PlotModel.InvalidatePlot(true);

        }

        private PlotModel _PlotModel = new PlotModel();

        public PlotModel PlotModel
        {
            get => _PlotModel;
            set => Set(ref _PlotModel, value);
        }

        public List<DataPoint> DataPoints { get; set; } = new List<DataPoint>();// { new DataPoint(1, 10), new DataPoint(2, 11), new DataPoint(3, 9) };

        //convertor from tuple to datapoinnts
        internal void UpdatePlotWithNewVals(IEnumerable<(double xx, double yy)> newVals, bool justClear = false)
        {
            DataPoints.Clear();
            if (!justClear)
                newVals.ToList().ForEach(c => DataPoints.Add(new DataPoint(c.xx, c.yy)));
            PlotModel.ResetAllAxes();//zoom to whole plot
            if (newVals?.Count()>0)
            {
                var newMax = newVals.Select(x => x.yy).Max(); newMax *= 1.1;
                PlotModel.Axes[1].Minimum = 0;
                PlotModel.Axes[1].Maximum = newMax;
            }
            PlotModel.InvalidatePlot(true);

            IsDataAvailableForPlot = DataPoints.Count > 0 ? true : false;
        }

        private bool _IsDataAvailableForPlot;//for displaying no data over plot

        public bool IsDataAvailableForPlot
        {
            get => _IsDataAvailableForPlot;
            set => Set(ref _IsDataAvailableForPlot, value);
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
            VideoMainFreq = -1;//means nothing
            UpdatePlotWithNewVals(null, true);
        }

        double _minrange;

        public double Minrange
        {
            get => _minrange;
            set => Set(ref _minrange, value);
        }

        double _maxrange;

        public double Maxrange
        {
            get => _maxrange;
            set => Set(ref _maxrange, value);
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
