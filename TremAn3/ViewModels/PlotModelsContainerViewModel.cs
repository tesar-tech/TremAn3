using GalaSoft.MvvmLight;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
    public class PlotModelsContainerViewModel:ViewModelBase
    {

        public PlotModelsContainerViewModel()
        {
            _PlotModelsProps =  GetType()
                 .GetProperties()
                 .Where(prop => prop.PropertyType == typeof(PlotModel))
                 .ToList();
        }

        private List<PropertyInfo> _PlotModelsProps;//this is useful when assigning something to properties

        public void SetAllModelsToNoData()
        {
            _PlotModelsProps.ForEach(x => x.SetValue(this, getPlotModelWithNoDataText()));
        }
        public void SetFreqProgressToNoData()
        {
            FreqProgressPlotModel = getPlotModelWithNoDataText();
        }

        public void InvalidateTimePlots(bool updateData)
        {
            TimePlotModels.ForEach(x => x.InvalidatePlot(updateData));

        }
        public void InvalidateAllPlots(bool updateData)
        {
            AllPlotModels.ForEach(x => x.InvalidatePlot(updateData));
        }

        private List<PlotModel> TimePlotModels
        {
            get =>
new List<PlotModel> { XCoMPlotModel, YCoMPlotModel, FreqProgressPlotModel }
;
        }


        public List<PlotModel> AllPlotModels { get=>
                new List<PlotModel> { PSDPlotModel,AmpSpecPlotModel,XCoMPlotModel,YCoMPlotModel,FreqProgressPlotModel}
                ; }

        private PlotModel _PSDPlotModel;

        public PlotModel PSDPlotModel
        {
            get => _PSDPlotModel;
            set => Set(ref _PSDPlotModel, value);
        }

        private PlotModel _AmpSpecPlotModel;

        public PlotModel AmpSpecPlotModel
        {
            get => _AmpSpecPlotModel;
            set => Set(ref _AmpSpecPlotModel, value);
        }



        private PlotModel _XCoMPlotModel;

        public PlotModel XCoMPlotModel
        {
            get => _XCoMPlotModel;
            set => Set(ref _XCoMPlotModel, value);
        }

        private PlotModel _YCoMPlotModel;

        public PlotModel YCoMPlotModel
        {
            get => _YCoMPlotModel;
            set => Set(ref _YCoMPlotModel, value);
        }

        private PlotModel _FreqProgressPlotModel;

        public PlotModel FreqProgressPlotModel
        {
            get => _FreqProgressPlotModel;
            set => Set(ref _FreqProgressPlotModel, value);
        }
        private PlotModel getPlotModelWithNoDataText()
        {
            return new PlotModel { Title = "No Data" };
        }
    }

}
