using GalaSoft.MvvmLight;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;

namespace TremAn3.ViewModels;

public class PlotModelsContainerViewModel : ViewModelBase
{

    public void SetAllModelsToNoData()
    {
        PlotModels.ToList().ForEach(x => x.PlotModel = getPlotModelWithNoDataText());
        PlotModelsGlobalScope.ToList().ForEach(x => x.PlotModel = getPlotModelWithNoDataText());
    }
    public void SetFreqProgressToNoData()
    {
        PlotModels.First(x => x.DataSeriesType == DataSeriesType.FreqProgress).PlotModel = getPlotModelWithNoDataText();
    }

    public void InvalidateTimePlots(bool updateData)
    {
        PlotModels.Where(x =>
                x.DataSeriesType is DataSeriesType.FreqProgress or DataSeriesType.ComX or DataSeriesType.ComY)
            .ToList().ForEach(x=>x.PlotModel.InvalidatePlot(updateData));
    }
    public void InvalidateAllPlots(bool updateData)
    {
        PlotModels.ToList().ForEach(x => x.PlotModel.InvalidatePlot(updateData));
        PlotModelsGlobalScope.ToList().ForEach(x => x.PlotModel.InvalidatePlot(updateData));
    }

    //there is one line series for every roi. For example amp spectrum
    public List<PlotModelWithTypeViewModel> PlotModels { get; set; } = [];


    //there is only one series for all rois. For example coherence
    public List<PlotModelWithTypeViewModel> PlotModelsGlobalScope { get; set; } = [];


    private PlotModel getPlotModelWithNoDataText()
    {
        return new PlotModel { Title = "No Data" };
    }

    private string currentDataPointString;
    //when moving annotation on plot, all plots change this.. waiting for the future
    public string CurrentDataPointString
    {
        get => currentDataPointString;
        private set => Set(ref currentDataPointString, value);
    }


    private string GetCurrentDataPoint(PlotModelWithTypeViewModel plotModel)
    {
        return plotModel?.CurrentDataPoint == null
            ? "no data point"
            : $"X: {plotModel.CurrentDataPoint.Value.X:0.00}, Y: {plotModel.CurrentDataPoint.Value.Y:0.00} ";
    }

    /// <summary>
    /// this needs to be done when new plotModel is presented.
    /// </summary>

    public PlotModel GetPlotModelByDsTypeOrCreateNew( DataSeriesType type)
    {
        var plotModel = PlotModels.SingleOrDefault(x => x.DataSeriesType == type);

        if (plotModel == null)//adding new plot models, when xaml asks for them
        {
            plotModel = new PlotModelWithTypeViewModel() { DataSeriesType = type };
            PlotModels.Add(plotModel);
        }
        if(plotModel.PlotModel!=null )
            plotModel.PlotModel.TrackerChanged += (sender, args) => {
                plotModel.CurrentDataPoint = args?.HitResult?.DataPoint;
                CurrentDataPointString = GetCurrentDataPoint(plotModel);
            };
        return plotModel.PlotModel;

    }

    public PlotModel GetGlobalScopedPlotModelByDsTypeOrCreateNew(DataSeriesType type)
    {
        var plotModel = PlotModelsGlobalScope.SingleOrDefault(x => x.DataSeriesType == type);

        if (plotModel == null)//adding new plot models, when xaml asks for them
        {
            plotModel = new PlotModelWithTypeViewModel() { DataSeriesType = type };
            PlotModelsGlobalScope.Add(plotModel);
        }
        return plotModel.PlotModel;

    }


}
