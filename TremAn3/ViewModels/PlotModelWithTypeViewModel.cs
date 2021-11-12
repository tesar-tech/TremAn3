using GalaSoft.MvvmLight;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;


namespace TremAn3.ViewModels
{
    public class PlotModelWithTypeViewModel : ViewModelBase
    {
        private PlotModel _PlotModel;

        public PlotModel PlotModel
        {
            get => _PlotModel;
            set => Set(ref _PlotModel, value);
        }

        //public PlotModel PlotModel { get; set; }

        public DataSeriesType DataSeriesType { get; set; }

    }

    public static class PlotModelCollectionExtensions
    {
        public static PlotModelWithTypeViewModel GetPlotModelByDsType(this ObservableCollection<PlotModelWithTypeViewModel> coll, DataSeriesType type)
        {

            var vm = coll.SingleOrDefault(x => x.DataSeriesType == type);
            if (vm == null)
                vm = new PlotModelWithTypeViewModel() { DataSeriesType = type };
            return vm;
        }
    }

}
