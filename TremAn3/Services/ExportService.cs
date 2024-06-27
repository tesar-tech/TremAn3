namespace TremAn3.Services;

using Core;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

public class ExportService
{
    public async void ExportComValuesAsync()
    {
        var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels.Where(x => x.IsShowInPlot && x.ComputationViewModel.HasResult).ToList();
        if (rois.Count == 0)
        {
            ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Nothing to export");
            return;
        }


        var oneX = rois[0].ComputationViewModel.Algorithm.Results.FrameTimes.Select(x => x.TotalSeconds).ToList();
        var comX_fromMultipleRois = rois.Select(x => x.ComputationViewModel.Algorithm.Results.DataResultsDict[DataSeriesType.ComY].Y).ToList();
        var comY_fromMultipleRois = rois.Select(y => y.ComputationViewModel.Algorithm.Results.DataResultsDict[DataSeriesType.ComX].Y).ToList();


        List<List<double>> combinedComXComY = new();
        for (int i = 0; i < comX_fromMultipleRois.Count; i++)
        {
            combinedComXComY.Add(comX_fromMultipleRois[i]);
            combinedComXComY.Add(comY_fromMultipleRois[i]);
        }


        List<string> yHeaders = new();
        foreach (SelectionRectangleViewModel t in rois)
        {
            yHeaders.Add($"ComX__{t}");
            yHeaders.Add($"ComY__{t}");
        }

        var str = CsvBuilder.GetCvsFromOneX_MultipleY(xs: oneX, multiple_ys: combinedComXComY, (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator),
        headers: yHeaders.Prepend("time[s]"));

        var mainViewModel = ViewModelLocator.Current.MainViewModel;
        var name = $"{mainViewModel.MediaPlayerViewModel.VideoPropsViewModel.DisplayName}_" +
            $"{ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.DateTime:yyyy-MM-dd_HH-mm-ss}_CoM_Values";
        var (status, newName) = await CsvExport.ExportStringAsCsvAsync(str, name);
        mainViewModel.NotifyBasedOnStatus(status, newName);
    }
    public async Task ExportFreqAnalysisToCsvAsync()
    {
        var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels.Where(x => x.IsShowInPlot && x.ComputationViewModel.HasResult).ToList();

        if (rois.Count == 0)
        {
            ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Nothing to export");
            return;
        }

        List<List<double>> values = [];
        List<string> headers = [];

        List<DataSeriesType> toExportDataSeries = [DataSeriesType.Psd, DataSeriesType.AmpSpec, DataSeriesType.Welch, DataSeriesType.FreqProgress];
        foreach (var dataSeriesType in toExportDataSeries)
        {
            var roi0DataResult = rois[0].ComputationViewModel.Algorithm.Results.DataResultsDict[dataSeriesType];
            if(!roi0DataResult.IsOk) continue;
            values.Add(roi0DataResult.X);
            headers.Add($"freq[Hz]_{dataSeriesType}");
            foreach (var roi in rois)
            {
                List<double> yValues = roi.ComputationViewModel.Algorithm.Results.DataResultsDict[dataSeriesType].Y;
                values.Add(yValues);
                headers.Add($"{dataSeriesType}__{roi}");
            }
        }
         var currentGlobal = ViewModelLocator.Current.MainViewModel.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel;
        var roi1Roi2Coherence = currentGlobal.DataResultsDict[DataSeriesType.Coherence];
        if (roi1Roi2Coherence.IsOk)
        {
            values.Add(roi1Roi2Coherence.X);
            headers.Add("freq[Hz]_coherence_roi1_roi2");

            values.Add(roi1Roi2Coherence.Y);
            headers.Add("coherence_roi1_roi2");


            values.Add(currentGlobal.CoherenceMeasurementResults.Select(x => x.MinHz).ToList());
            headers.Add("coherence_minFreq[Hz]");

            values.Add(currentGlobal.CoherenceMeasurementResults.Select(x => x.MaxHz).ToList());
            headers.Add("coherence_maxFreq[Hz]");

            values.Add(currentGlobal.CoherenceMeasurementResults.Select(x => x.Average).ToList());
            headers.Add("coherence_average");

            // values.Add([ViewModelLocator.Current.MainViewModel.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel.CoherenceAverage]);
            // headers.Add("coherence_roi1_roi2_average");

        }

        var str = CsvBuilder.GetCsvFromListOfLists(values: values,
        (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator), headers: headers);
        var mainViewModel = ViewModelLocator.Current.MainViewModel;

        var name = $"{mainViewModel.MediaPlayerViewModel.VideoPropsViewModel.DisplayName}_" +
            $"{ViewModelLocator.Current.PastMeasurementsViewModel.SelectedMeasurementVm.DateTime:yyyy-MM-dd_HH-mm-ss}_freqAnalysis";
        var (status, newName) = await CsvExport.ExportStringAsCsvAsync(str, name);
        mainViewModel.NotifyBasedOnStatus(status, newName);
    }
}
