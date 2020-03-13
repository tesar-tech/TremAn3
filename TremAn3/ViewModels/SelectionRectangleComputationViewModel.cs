using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TremAn3.Core;

namespace TremAn3.ViewModels
{
    public class SelectionRectangleComputationViewModel:ViewModelBase
    {


        public SelectionRectangleComputationViewModel(Color color)
        {
            this.color = OxyColor.FromArgb(255, color.R, color.G, color.B);
            
        }
        internal CenterOfMotionAlgorithm InitializeCoM(int decodedPixelWidth, int decodedPixelHeight, double frameRate, SelectionRectangle rectangle)
        {
            Algorithm = new CenterOfMotionAlgorithm(decodedPixelWidth, decodedPixelHeight, frameRate, rectangle);
            return Algorithm;
        }
        public CenterOfMotionAlgorithm   Algorithm{ get; set; }

        public void PrepareForDisplay()
        {
            MainFreq = Algorithm.GetMainFreqAndFillPsdDataFromComLists();
            
            PsdSeries = new LineSeries
            {
                ItemsSource = Algorithm.Results.PsdAvgData.Select(c => new DataPoint(c.x_freq, c.y_power)),
                Color = color
            };

            XComSeries = new LineSeries
            {
                ItemsSource = Algorithm.Results.ListComXNoAvg.Select((val,i) => new DataPoint(i, val)),
                Color = color
            };

            YComSeries = new LineSeries
            {
                ItemsSource = Algorithm.Results.ListComXNoAvg.Select((val, i) => new DataPoint(i, val)),
                Color = color
            };


        }
        private OxyColor color;

        private LineSeries _PsdSeries;

        public LineSeries PsdSeries
        {
            get => _PsdSeries;
            set => Set(ref _PsdSeries, value);
        }


        private LineSeries _XComSeries;

        public LineSeries XComSeries
        {
            get => _XComSeries;
            set => Set(ref _XComSeries, value);
        }

        private LineSeries _YComSeries;

        public LineSeries YComSeries
        {
            get => _YComSeries;
            set => Set(ref _YComSeries, value);
        }



        private double _MainFreq;
   

        public double MainFreq
        {
            get => _MainFreq;
            set => Set(ref _MainFreq, value);
        }


    }
}
