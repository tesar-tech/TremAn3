using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using Windows.UI.Core;

namespace TremAn3.ViewModels;

using OxyPlot;
using Services;
using System.Collections.ObjectModel;
using TremAn3.Views;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

public class ResultsViewModel : ViewModelBase
{


    public ResultsViewModel()
    {
        CoherenceMeasurementResults.CollectionChanged += async (_, e) =>
        {

            if (AddedToCollection == null)
                return;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddedToCollection.Invoke();
            }

        };
    }


    public Action AddedToCollection { get; set; }


    //setups the result from model, then allow computation. Wout this, computation run on data loading (and rewrite files)
    public bool IsComputationPaused { get; set; } = true;

    /// <summary>
    /// computes all the results and place it into the DataResutlsDict
    /// </summary>
    /// <param name="frameRate"></param>
    /// <param name="comXAllRois"></param>
    /// <param name="comYAllRois"></param>
    public async Task ComputeAllResults(double frameRate, List<List<double>> comXAllRois, List<List<double>> comYAllRois)
    {
        await Task.Run(async () =>
        {
            Coherence coherence = new((int)frameRate, comXAllRois, comYAllRois);

            DataResultsDict.Clear();
            var coh = coherence.Compute();
            DataResultsDict.Add(DataSeriesType.Coherence, coh);
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                IsCoherenceOk = coh.IsOk;
                IsComputationPaused = true;
                CoherenceMinHz = 0;
                CoherenceMaxHz = frameRate / 2;
                await ComputeAdditionalResults();
                IsComputationPaused = false;
            });
        });
    }

    private bool _isCoherenceOk;
    public bool IsCoherenceOk
    {
        get => _isCoherenceOk;
        set => Set(ref _isCoherenceOk, value);
    }



    public async Task ComputeAdditionalResults()
    {

        await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        {
            if (ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm == null)
                return;
            if (!DataResultsDict.ContainsKey(DataSeriesType.Coherence) || DataResultsDict[DataSeriesType.Coherence].Y == null)
                return;
            var r = DataResultsDict[DataSeriesType.Coherence];
            //find indexes of r.X where r.X is between CoherenceMinHz and CoherenceMaxHz
            var minIndex = r.X.IndexOf(r.X.FirstOrDefault(x => x >= CoherenceMinHz));
            var maxIndex = r.X.IndexOf(r.X.LastOrDefault(x => x <= CoherenceMaxHz));
            //get r.Y values between minIndex and maxIndex

            if (minIndex == -1 || maxIndex == -1)
                return;
            if (minIndex >= maxIndex)
                return;
            var chAverage = r.Y.GetRange(minIndex, maxIndex - minIndex).Average();
            CoherenceMeasurementResults.Add(new() { Average = chAverage, MaxHz = CoherenceMaxHz, MinHz = CoherenceMinHz });
            await ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.SaveAdditionalResults();

        });
    }


    private ObservableCollection<CoherenceMeasurementResults> _coherenceMeasurementResults = [];
    public ObservableCollection<CoherenceMeasurementResults> CoherenceMeasurementResults
    {
        get => _coherenceMeasurementResults;
        //set => Set(ref _coherenceMeasurementResults, value);
    }

    public async Task RemoveOne(CoherenceMeasurementResults res)
    {
        CoherenceMeasurementResults.Remove(res);
        await ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.SaveAdditionalResults();
    }


    public AdditionalResultsModel GetAdditionalResultsModel()
    {
        AdditionalResultsModel additionalResultsModel = new();

        additionalResultsModel.CoherenceAverageResults =
             CoherenceMeasurementResults.Select(x => new CoherenceAverageResultModel() { Average = x.Average, MaxHz = x.MaxHz, MinHz = x.MinHz })
             .ToList();

        additionalResultsModel.PointsCollectors = PointsCollectors.Select(x => new PointsCollectorModel()
        {
            DataSeriesType = x.Key,
            Points = x.Value.Points.Select(y => new PointToCollectModel() { X = y.X, Ys = y.Ys }).ToList()
        }).ToList();

        return additionalResultsModel;

    }

    public bool isSettingRangeDontCompute;

    private double _CoherenceMinHz;
    public double CoherenceMinHz
    {
        get => _CoherenceMinHz;
        set => Set(ref _CoherenceMinHz, value);

    }
    private double _CoherenceMaxHz;
    public double CoherenceMaxHz
    {
        get => _CoherenceMaxHz;
        set => Set(ref _CoherenceMaxHz, value);
    }


    public Dictionary<DataSeriesType, PointsCollectorVm> PointsCollectors { get; set; } = new() {
        { DataSeriesType.Psd, new()},
        { DataSeriesType.AmpSpec, new()},
        { DataSeriesType.Welch, new()},
        };

    public PointsCollectorVm GetPointsCollectorVm(DataSeriesType type)
    {
        return PointsCollectors[type];
    }


    public async void DeleteAllMeasurements()
    {
        CoherenceMeasurementResults.Clear();
        await ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.SaveAdditionalResults();

    }



    public async Task CoherenceRangeChanged()
    {
        if (!IsComputationPaused)
        {
            await ComputeAdditionalResults();//coherence average
        }
        await ViewModelLocator.Current.FreqCounterViewModel.DisplayPlots(false);
        // await FreqCounterViewModel.ParentVm.PastMeasurementsViewModel.SaveSelectedMeasurement();
    }

    internal void ResetResults()
    {
        isSettingRangeDontCompute = true;
        CoherenceMeasurementResults.Clear();
        isSettingRangeDontCompute = false;
    }

    internal void SetIsCoherenceOk()
    {
        if (DataResultsDict.ContainsKey(DataSeriesType.Coherence))
            IsCoherenceOk = DataResultsDict[DataSeriesType.Coherence].IsOk;
        else
            IsCoherenceOk = false;
    }

    public Dictionary<DataSeriesType, DataResult> DataResultsDict
    {
        get;
        set;
    } = [];

    //public Guid Id { get; set; } = Guid.Empty;
}

public class CoherenceMeasurementResults : ViewModelBase
{


    private double average;
    public double Average
    {
        get => average;
        set => Set(ref average, value);
    }

    private double _minHz;
    public double MinHz
    {
        get => _minHz;
        set => Set(ref _minHz, value);
    }

    private double _maxHz;
    public double MaxHz
    {
        get => _maxHz;
        set => Set(ref _maxHz, value);
    }
    public async void DeleteMe(object sender, RoutedEventArgs e)
    {
        await ViewModelLocator.Current.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel.RemoveOne(this);
    }
}

