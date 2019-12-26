using GalaSoft.MvvmLight;
using OxyPlot;
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
            if(!justClear)
            newVals.ToList().ForEach(c => DataPoints.Add(new DataPoint(c.xx, c.yy)));
            PlotModel.InvalidatePlot(true);
                PlotModel.ResetAllAxes();//zoom to whole plot
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

        private double _percentageOfResolution = 100;

        public double PercentageOfResolution
        {
            get => _percentageOfResolution;
            set => Set(ref _percentageOfResolution, value);
        }




    }
}
