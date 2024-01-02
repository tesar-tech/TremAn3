using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using Windows.UI.Core;

namespace TremAn3.ViewModels;

using Services;
using Windows.ApplicationModel.Core;

public class ResultsViewModel : ViewModelBase
{
    /// <summary>
    /// computes all the results and place it into the DataResutlsDict
    /// </summary>
    /// <param name="frameRate"></param>
    /// <param name="comXAllRois"></param>
    /// <param name="comYAllRois"></param>
    public async Task ComputeAllResults(double frameRate, List<List<double>> comXAllRois, List<List<double>> comYAllRois)
    {
        await Task.Run(async () => {
            Coherence coherence = new Coherence((int)frameRate, comXAllRois, comYAllRois);
            DataResultsDict.Clear();
            var coh = coherence.Compute();
            DataResultsDict.Add(DataSeriesType.Coherence, coh);
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                await ComputeAdditionalResults();
            });
        });
    }

    public async Task ComputeAllResults(double frameRate, List<List<double>> comXAllRois, List<List<double>> comYAllRois, List<double> freqProgress)
    {
        await Task.Run(async () => {
            Coherence coherence = new((int)frameRate, comXAllRois, comYAllRois);
            DataResultsDict.Clear();
            var coh = coherence.Compute();
            DataResultsDict.Add(DataSeriesType.Coherence, coh);
            await ComputeAdditionalResults();
        });
    }

    public async Task ComputeAdditionalResults()
    {
        await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            if (DataResultsDict.ContainsKey(DataSeriesType.Coherence) && DataResultsDict[DataSeriesType.Coherence].Y != null)
                CoherenceAverage = DataResultsDict[DataSeriesType.Coherence].Y.Average();
        });
    }

    private double coherenceAverage;

    public double CoherenceAverage
    {
        get => coherenceAverage;
        set => Set(ref coherenceAverage, value);
    }


    public Dictionary<DataSeriesType, DataResult> DataResultsDict { get; set; } = new();

    //public Guid Id { get; set; } = Guid.Empty;
}
