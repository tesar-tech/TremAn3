using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TremAn3.Core;

namespace TremAn3.ViewModels
{
    public class SelectionRectangleComputationViewModel : ViewModelBase
    {


        public SelectionRectangleComputationViewModel(Color color, SelectionRectangleViewModel parent)
        {
            (this.color, this.parent) = (OxyColor.FromArgb(255, color.R, color.G, color.B), parent);
            FreqCounterViewModel.IsRoiSameAsResultSomeChange(false);//obsolete result, when new roi is added
        }

        private SelectionRectangleViewModel parent { get; set; }
        public FreqCounterViewModel FreqCounterViewModel { get => ViewModelLocator.Current.FreqCounterViewModel; }


        private bool _IsRoiSameAsResult;

        //if roi is moved, result doesnt meet the roi, so result is deleted
        //setting to false will delete plots 
        public bool IsRoiSameAsResult
        {
            get => _IsRoiSameAsResult;
            set
            {
                if (Set(ref _IsRoiSameAsResult, value))//no need for notify property changed
                {
                    if (!value)
                    {
                        Algorithm = null;
                        RaisePropertyChanged(nameof(HasResult));
                        PsdSeries.PlotModel.Series.Remove(PsdSeries);
                        XComSeries.PlotModel.Series.Remove(XComSeries);
                        YComSeries.PlotModel.Series.Remove(YComSeries);
                        PsdSeries = XComSeries = YComSeries = null;
                        parent.PlotsNeedRefresh.Invoke();
                    }
                    FreqCounterViewModel.IsRoiSameAsResultSomeChange(value);
                }
            }
        }

        public bool HasResult { get => Algorithm?.Results != null; }
        internal CenterOfMotionAlgorithm InitializeCoM(int decodedPixelWidth, int decodedPixelHeight, double frameRate, SelectionRectangle rectangle)
        {
            Algorithm = new CenterOfMotionAlgorithm(decodedPixelWidth, decodedPixelHeight, frameRate, rectangle);
            RaisePropertyChanged(nameof(HasResult));
            return Algorithm;
        }
        public CenterOfMotionAlgorithm Algorithm { get; set; }

        //run neccessary computations and create lineseries
        public void PrepareForDisplay(int stepForFreqProgress, int segmnetSizeFreqProgress)
        {
            MainFreq = Algorithm.GetMainFreqAndFillPsdDataFromComLists();
            PsdSeries = GetNewLineSeries(Algorithm.Results.PsdAvgData.Select(c => new DataPoint(c.x_freq, c.y_power)));
            XComSeries = GetNewLineSeries(Algorithm.Results.ListComXNoAvg.Zip(Algorithm.Results.FrameTimes, (valy, valx) => new DataPoint(valx.TotalSeconds, valy)));
            YComSeries = GetNewLineSeries(Algorithm.Results.ListComYNoAvg.Zip(Algorithm.Results.FrameTimes, (valy, valx) => new DataPoint(valx.TotalSeconds, valy)));
            //PrepareForDisplayFreqProgress(stepForFreqProgress, segmnetSizeFreqProgress);
            IsRoiSameAsResult = true;
        }

        public void PrepareForDisplayFreqProgress(int step, int segmnetSizeFreqProgress)
        {
           Algorithm.GetFftDuringSignal(segmnetSizeFreqProgress, step);//todo check inside, it has to compute lists firs
           FreqProgressSeries =GetNewLineSeries(Algorithm.Results.FreqProgress.Zip(Algorithm.Results.FreqProgressTime, (valy, valx) => new DataPoint(valx, valy)));

        }


        private LineSeries GetNewLineSeries(IEnumerable itemSource) => new LineSeries
        {
            ItemsSource = itemSource,
            Color = color,
            StrokeThickness = parent.IsShowInPlot ? defaultStrokeThickness : notShownStrokeThickness
        };

        readonly double defaultStrokeThickness = 0.75;
        readonly double notShownStrokeThickness = 0.08;
        private readonly OxyColor color;

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

        private LineSeries _FreqProgressSeries;

        public LineSeries FreqProgressSeries
        {
            get => _FreqProgressSeries;
            set => Set(ref _FreqProgressSeries, value);
        }


        private double _MainFreq;

        public double MainFreq
        {
            get => _MainFreq;
            set => Set(ref _MainFreq, value);
        }

        //if this result is not shown in plot, thickness of line will be lowerd
        internal void ChangeVisibilityOfLines(bool isShowInPlot)
        {
            var thickness = isShowInPlot ? defaultStrokeThickness : notShownStrokeThickness;//when unvisible, just change thickness to small value
            PsdSeries.StrokeThickness = XComSeries.StrokeThickness = YComSeries.StrokeThickness = thickness;
        }
    }
}
