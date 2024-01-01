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
        }

        public async Task IsRoiSameAsResultSet(bool value)
        {
            if (Set(ref _IsRoiSameAsResult, value))//no need for notify property changed
            {
                if (!value)
                {
                    Algorithm = null;
                    RaisePropertyChanged(nameof(HasResult));
                    LineSeriesDict.ToList().ForEach(x => { x.Value.PlotModel.Series.Remove(x.Value); });
                    LineSeriesDict.Clear();
                    //LineSeriesProps.ForEach(x => x.SetValue(this, null));
                    parent.PlotsNeedRefresh.Invoke();
                    //FreqCounterViewModel.PlotModelsContainerViewModel.InvalidateAllPlots(true);
                    await ViewModelLocator.Current.PastMeasurementsViewModel.RoiIsNotSameAsResult();
                }
                FreqCounterViewModel.IsRoiSameAsResultSomeChange(value);
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
        public async Task PrepareForDisplay(int stepForFreqProgress, int segmnetSizeFreqProgress, bool doComptutation)
        {
            //MainFreq = Algorithm.GetMainFreqAndFillPsdDataFromComLists();
            //Algorithm.FillAmpSpectDataFromComLists();
            if (doComptutation)
            {
                Algorithm.segmentSize = segmnetSizeFreqProgress;
                Algorithm.step = stepForFreqProgress;
                await Algorithm.ComputeAllData();
            }
            LineSeriesDict.Clear();//when you dont move roi, it is the same with previous results
            foreach (var res in Algorithm.Results.DataResultsDict)
            {
                if (res.Value.IsOk)
                    LineSeriesDict.Add(res.Key, GetNewLineSeries(res.Value.X.Zip(res.Value.Y, (x, y) => new DataPoint(x, y))));
            }
            await IsRoiSameAsResultSet(true);
        }

        public async Task PrepareForDisplay(DataSeriesType dataSeriesType, int stepForFreqProgress = 0, int segmnetSizeFreqProgress = 0)
        {
            Algorithm.segmentSize = segmnetSizeFreqProgress;
            Algorithm.step = stepForFreqProgress;
            await Algorithm.ComputeOnly(dataSeriesType);
            var res = Algorithm.Results.DataResultsDict[dataSeriesType];
            if (res.IsOk)
                LineSeriesDict[dataSeriesType] = GetNewLineSeries(res.X.Zip(res.Y, (x, y) => new DataPoint(x, y)));
            //await IsRoiSameAsResultSet(true);
        }

        //public void PrepareForDisplayFreqProgress(int step, int segmnetSizeFreqProgress)
        //{
        //    Algorithm.GetFftDuringSignal(segmnetSizeFreqProgress, step);//todo check inside, it has to compute lists firs
        //    LineSeriesDict[DataSeriesType.FreqProgress] = GetNewLineSeries(Algorithm.Results.FreqProgress.Zip(Algorithm.Results.FreqProgressTime, (valy, valx) => new DataPoint(valx, valy)));
        //}

        public Dictionary<DataSeriesType, LineSeries> LineSeriesDict { get; set; } = new Dictionary<DataSeriesType, LineSeries>();

        private LineSeries GetNewLineSeries(IEnumerable itemSource) => new LineSeries
        {
            ItemsSource = itemSource,
            Color = color,
            TrackerFormatString = "{0}\n{1}: {2:0.000}\n{3}: {4:0.000}",
            StrokeThickness = parent.IsShowInPlot ? defaultStrokeThickness : notShownStrokeThickness
        };

        readonly double defaultStrokeThickness = 0.75;
        readonly double notShownStrokeThickness = 0.08;
        private readonly OxyColor color;



        internal void ChangeVisibilityOfLines(bool isShowInPlot)
        {
            var thickness = isShowInPlot ? defaultStrokeThickness : notShownStrokeThickness;//when unvisible, just change thickness to small value
            LineSeriesDict.ToList().ForEach(x => x.Value.StrokeThickness = thickness);
        }
    }
}
