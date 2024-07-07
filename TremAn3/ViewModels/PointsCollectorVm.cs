using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;

namespace TremAn3.ViewModels;

public class PointsCollectorVm : ViewModelBase
{
    public PointsCollectorVm()
    {
        Points.CollectionChanged += async (_, e) =>
        {

            if (ViewModelLocator.Current.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel.IsComputationPaused) return;
            await ViewModelLocator.Current.FreqCounterViewModel.DisplayPlots(false);
            await ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.SaveAdditionalResults();
        };
    }

    private ObservableCollection<PointToCollectVm> _points = [];
    public ObservableCollection<PointToCollectVm> Points
    {
        get => _points;
        set => Set(ref _points, value);
    }


    public async Task AddPoint(double point)
    {
        Points.Add(new(point, [], this));
    }

    


    private bool _isInCollectingState ;
    public bool IsInCollectingState
    {
        get => _isInCollectingState;
        set => Set(ref _isInCollectingState, value);
    }
    public async void DeleteAllPoints()
    {
        Points.Clear();
    }

    internal async Task DeletePoint(PointToCollectVm pointToCollect)
    {
        Points.Remove(pointToCollect);
    }


    private string _collectingText = "Collect points";
    public string CollectingText
    {
        get => _collectingText;
        set => Set(ref _collectingText, value);
    }


    public void ChangeCollecting()
    {

        IsInCollectingState = !IsInCollectingState;
        CollectingText = IsInCollectingState ? "Stop collecting" : "Collect points";
    }


}

public class PointToCollectVm : ViewModelBase
{
    public PointToCollectVm(double x,List<double> ys, PointsCollectorVm parent)
    {
        X = x;
        Ys = ys;
        Parent = parent;
    }
    private double _x;
    public double X
    {
        get => _x;
        set => Set(ref _x, value);
    }

    private List<double> _ys;

    //for every roi, order does matter
    public List<double> Ys
    {
        get => _ys;
        set => Set(ref _ys, value);
    }

    public PointsCollectorVm Parent { get; }

    public async void DeleteMe()
    {
        await Parent.DeletePoint(this);
    }
}

