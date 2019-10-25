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
        internal void UpdatePlotWithNewVals(IEnumerable<(int xx, int yy)> newVals)
        {
            DataPoints.Clear();
            newVals.ToList().ForEach(c => DataPoints.Add(new DataPoint(c.xx, c.yy)));
            PlotModel.InvalidatePlot(true);
            PlotModel.ResetAllAxes();//zoom to whole plot
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

        public string DoubleToTimeConverter(double time)
        {
            TimeSpan result = TimeSpan.FromSeconds(time);
            string TimeString = result.ToString("mm':'ss");
            return TimeString;
        }
    }
}
