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

namespace TremAn3.ViewModels
{
    public class PlotModelsContainerViewModel : ViewModelBase
    {

        public PlotModelsContainerViewModel()
        {
            //_PlotModelsProps = GetType()
            //     .GetProperties()
            //     .Where(prop => prop.PropertyType == typeof(PlotModel))
            //     .ToList();

            //PlotModels.Add(new PlotModelWithTypeViewModel() { DataSeriesType=Core.DataSeriesType.AmpSpec });
        }

        //private List<PropertyInfo> _PlotModelsProps;//this is useful when assigning something to properties

        public void SetAllModelsToNoData()
        {
            PlotModels.ToList().ForEach(x => x.PlotModel = getPlotModelWithNoDataText());
        }
        public void SetFreqProgressToNoData()
        {
            PlotModels.First(x => x.DataSeriesType == DataSeriesType.FreqProgress).PlotModel = getPlotModelWithNoDataText();
        }

        public void InvalidateTimePlots(bool updateData)
        {
            PlotModels.Where(x =>
            x.DataSeriesType == DataSeriesType.FreqProgress ||
            x.DataSeriesType == DataSeriesType.ComX ||
            x.DataSeriesType == DataSeriesType.ComY)
            .ToList().ForEach(x=>x.PlotModel.InvalidatePlot(updateData));
            //TimePlotModels.ForEach(x => x.InvalidatePlot(updateData));
        }
        public void InvalidateAllPlots(bool updateData)
        {
            PlotModels.ToList().ForEach(x => x.PlotModel.InvalidatePlot(updateData));
        }

        public ObservableCollection<PlotModelWithTypeViewModel> PlotModels { get; set; } =
            new ObservableCollection<PlotModelWithTypeViewModel>();


        private PlotModel getPlotModelWithNoDataText()
        {
            return new PlotModel { Title = "No Data" };
        }



        /// <summary>
        /// this needs to be done when new plotModel is presented. Don't want to do it 
        /// </summary>


        public PlotModel GetPlotModelByDsTypeOrCreateNew( DataSeriesType type)
        {
            var plotModel = PlotModels.SingleOrDefault(x => x.DataSeriesType == type);

            if (plotModel == null)
            {
                plotModel = new PlotModelWithTypeViewModel() { DataSeriesType = type };
                PlotModels.Add(plotModel);
            }
            return plotModel.PlotModel;

        }
    }

}
