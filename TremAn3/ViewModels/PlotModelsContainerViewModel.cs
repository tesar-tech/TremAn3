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
            _PlotModelsProps = GetType()
                 .GetProperties()
                 .Where(prop => prop.PropertyType == typeof(PlotModel))
                 .ToList();

            //PlotModels.Add(new PlotModelWithTypeViewModel() { DataSeriesType=Core.DataSeriesType.AmpSpec });
        }

        private List<PropertyInfo> _PlotModelsProps;//this is useful when assigning something to properties

        public void SetAllModelsToNoData()
        {
            _PlotModelsProps.ForEach(x => x.SetValue(this, getPlotModelWithNoDataText()));
            PlotModels.ToList().ForEach(x => x.PlotModel = getPlotModelWithNoDataText());
           RaisePlotModelsPropChange();
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
            PlotModels.ToList().ForEach(x => x.PlotModel.InvalidatePlot(updateData));
            AllPlotModels.ForEach(x => x.InvalidatePlot(updateData));
        }

        public ObservableCollection<PlotModelWithTypeViewModel> PlotModels { get; set; } =
            new ObservableCollection<PlotModelWithTypeViewModel>();

        private List<PlotModel> TimePlotModels
        {
            get =>
new List<PlotModel> { XCoMPlotModel, YCoMPlotModel, FreqProgressPlotModel }
;
        }


        public List<PlotModel> AllPlotModels
        {
            get =>
new List<PlotModel> { PSDPlotModel, XCoMPlotModel, YCoMPlotModel, FreqProgressPlotModel }
;
        }

        private PlotModel _PSDPlotModel;

        [Helpers.PlotName("Psd")]
        public PlotModel PSDPlotModel
        {
            get => _PSDPlotModel;
            set => Set(ref _PSDPlotModel, value);
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


        public void SetPlotModel(DataSeriesType type, PlotModel newPlotModel)
        {
            var oldPlotModel = PlotModels.FirstOrDefault(x => x.DataSeriesType == type);
            if (oldPlotModel != null)
                oldPlotModel.PlotModel = newPlotModel;

        }

        /// <summary>
        /// this needs to be done when new plotModel is presented. Don't want to do it 
        /// </summary>
        public void RaisePlotModelsPropChange() => RaisePropertyChanged(nameof(PlotModels));


        public PlotModel GetPlotModelByDsTypeOrCreateNew(ObservableCollection<PlotModelWithTypeViewModel> coll, DataSeriesType type)
        {

            var plotModel = coll.SingleOrDefault(x => x.DataSeriesType == type);

            if (plotModel == null)
            {
                plotModel = new PlotModelWithTypeViewModel() { DataSeriesType = DataSeriesType.AmpSpec };
                PlotModels.Add(plotModel);
            }
            return plotModel.PlotModel;

        }
    }

}
