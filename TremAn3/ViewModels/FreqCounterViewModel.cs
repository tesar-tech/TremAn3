﻿using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
    public class FreqCounterViewModel:ViewModelBase
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

        public List<DataPoint> DataPoints { get; set; } = new List<DataPoint>() { new DataPoint(1, 10), new DataPoint(2, 11), new DataPoint(3, 9) };

        //convertor from tuple to datapoinnts
        internal void UpdatePlotWithNewVals(IEnumerable<(int xx, int yy)> newVals)
        {
            DataPoints.Clear();
            newVals.ToList().ForEach(c => DataPoints.Add(new DataPoint(c.xx, c.yy)));
            PlotModel.InvalidatePlot(true);
        }
    }
}
