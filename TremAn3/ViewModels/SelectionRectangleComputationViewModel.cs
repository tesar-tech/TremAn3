using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                        LineSeriesDict.ToList().ForEach(x => { x.Value.PlotModel.Series.Remove(x.Value);});
                        LineSeriesDict.Clear();
                        //LineSeriesProps.ForEach(x => x.SetValue(this, null));
                        parent.PlotsNeedRefresh.Invoke();
                        //FreqCounterViewModel.PlotModelsContainerViewModel.InvalidateAllPlots(true);
                        ViewModelLocator.Current.PastMeasurementsViewModel.RoiIsNotSameAsResult();
                    }
                    FreqCounterViewModel.IsRoiSameAsResultSomeChange(value);
                }
            }
        }

        public bool HasResult { get => Algorithm?.Results != null; }
        internal void InitializeCoM(int decodedPixelWidth, int decodedPixelHeight, double frameRate, SelectionRectangle rectangle)
        {
            Algorithm = new CenterOfMotionAlgorithm(decodedPixelWidth, decodedPixelHeight, frameRate, rectangle);
            RaisePropertyChanged(nameof(HasResult));
        }
     
        public CenterOfMotionAlgorithm Algorithm { get; set; }

        //run neccessary computations and create lineseries
        public void PrepareForDisplay(int stepForFreqProgress, int segmnetSizeFreqProgress)
        {
            //MainFreq = Algorithm.GetMainFreqAndFillPsdDataFromComLists();
            //Algorithm.FillAmpSpectDataFromComLists();

            Algorithm.ComputeAllData();

            foreach (var res in Algorithm.Results.DataResultsDict)
            {
                LineSeriesDict.Add(res.Key, GetNewLineSeries( res.Value.X.Zip(res.Value.Y,(x,y) => new DataPoint(x, y)) )  );
            }

            //AmpSpecSeries = GetNewLineSeries(Algorithm.Results.AmpSpecData.Frequencies
            //    .Zip(Algorithm.Results.AmpSpecData.Values, (freq, value) => new DataPoint(freq, value)));
            //PsdSeries = GetNewLineSeries(Algorithm.Results.PsdAvgData.Frequencies
            //    .Zip(Algorithm.Results.PsdAvgData.Values, (freq, value) => new DataPoint(freq, value)));
            //XComSeries = GetNewLineSeries(Algorithm.Results.ListComXNoAvg.Zip(Algorithm.Results.ResultsModel.FrameTimes, (valy, valx) => new DataPoint(valx.TotalSeconds, valy)));
            //YComSeries = GetNewLineSeries(Algorithm.Results.ListComYNoAvg.Zip(Algorithm.Results.ResultsModel.FrameTimes, (valy, valx) => new DataPoint(valx.TotalSeconds, valy)));


            //PrepareForDisplayFreqProgress(stepForFreqProgress, segmnetSizeFreqProgress);
            IsRoiSameAsResult = true;
        }
        
        public void PrepareForDisplayFreqProgress(int step, int segmnetSizeFreqProgress)
        {
            Algorithm.GetFftDuringSignal(segmnetSizeFreqProgress, step);//todo check inside, it has to compute lists firs
            LineSeriesDict[DataSeriesType.FreqProgress] = GetNewLineSeries(Algorithm.Results.FreqProgress.Zip(Algorithm.Results.FreqProgressTime, (valy, valx) => new DataPoint(valx, valy)));
        }

        public Dictionary<DataSeriesType, LineSeries> LineSeriesDict { get; set; } = new Dictionary<DataSeriesType, LineSeries>();

        private LineSeries GetNewLineSeries(IEnumerable itemSource) => new LineSeries
        {
            ItemsSource = itemSource,
            Color = color,
            StrokeThickness = parent.IsShowInPlot ? defaultStrokeThickness : notShownStrokeThickness
        };

        readonly double defaultStrokeThickness = 0.75;
        readonly double notShownStrokeThickness = 0.08;
        private readonly OxyColor color;


        //private List<LineSeries> LineSeriesValues
        //{
        //    get
        //    {
        //        return GetType().GetProperties().Where(prop => prop.PropertyType == typeof(LineSeries))
        //                .Select(x => (LineSeries)x.GetValue(this)).ToList();
        //    }
        //}
        //private List<System.Reflection.PropertyInfo> LineSeriesProps
        //{
        //    get
        //    {
        //        return GetType().GetProperties().Where(prop => prop.PropertyType == typeof(LineSeries)).ToList();
        //    }
        //}


        //public LineSeries PsdSeries { get; set; }

        ////public LineSeries AmpSpecSeries { get; set; }
        

        //public LineSeries XComSeries { get; set; }


        //public LineSeries YComSeries { get; set; }


        //public LineSeries FreqProgressSeries { get; set; }


        //private double _MainFreq;

        //public double MainFreq
        //{
        //    get => _MainFreq;
        //    set => Set(ref _MainFreq, value);
        //}

        //if this result is not shown in plot, thickness of line will be lowerd
        internal void ChangeVisibilityOfLines(bool isShowInPlot)
        {
            var thickness = isShowInPlot ? defaultStrokeThickness : notShownStrokeThickness;//when unvisible, just change thickness to small value
            LineSeriesDict.ToList().ForEach(x => x.Value.StrokeThickness = thickness);
        }
    }
}
