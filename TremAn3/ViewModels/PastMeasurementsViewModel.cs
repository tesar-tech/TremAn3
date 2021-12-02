using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.Helpers;
using TremAn3.Services;
using Windows.Storage;
using Windows.UI.Core;

namespace TremAn3.ViewModels
{
    public class PastMeasurementsViewModel : ViewModelBase
    {

        public PastMeasurementsViewModel(MeasurementsService sms, DataService ds)
        {
            _StoringMeasurementsService = sms;
            _DataService = ds;
        }

        private MeasurementsService _StoringMeasurementsService;
        private DataService _DataService;
        public bool IsAllMeasurementsLoaded { get; set; }


        private bool _IsPastMeasurementsOpen;

        public bool IsPastMeasurementsOpen
        {
            get => _IsPastMeasurementsOpen;
            set
            {
                ButtonHideOrShowText = value ? "Hide" : "Show measurements";
                Set(ref _IsPastMeasurementsOpen, value);
            }
        }

        public void ChangeIsPastMeOpen()
        {
            IsPastMeasurementsOpen = !IsPastMeasurementsOpen;
        }

        private string _ButtonHideOrShowText = "Show measurements";

        public string ButtonHideOrShowText
        {
            get => _ButtonHideOrShowText;
            set => Set(ref _ButtonHideOrShowText, value);
        }

        private ObservableCollection<MeasurementViewModel> _MeasurementsVms = new ObservableCollection<MeasurementViewModel>();

        /// <summary>
        /// when the result is changed (roi is moved) it does not fit current measurement, thus is unselected
        /// </summary>
        internal async Task RoiIsNotSameAsResult()
        {
            await SelectedMeasurementVmSet(null);
        }

        public ObservableCollection<MeasurementViewModel> MeasurementsVms
        {
            get => _MeasurementsVms;
            set => Set(ref _MeasurementsVms, value);
        }

        private MeasurementViewModel _SelectedMeasurementVm;

        public MeasurementViewModel SelectedMeasurementVm => _SelectedMeasurementVm;//set is inside method

        public async Task SelectedMeasurementVmSet(MeasurementViewModel value, bool isBasedOnViewModel = false)
        {

            if (!Set(ref _SelectedMeasurementVm, value)) return;

            if (value == null)
            {
                return;
            }
            ViewModelLocator.Current.MainViewModel.IsDoingSomethingImportant = true;
            await Task.Delay(1);
            if (!value.IsVectorDataLoaded)
            {
                await _DataService.LoadVectorDataToModel(value.Model, value.FolderForMeasurement);
                value.IsVectorDataLoaded = true;
            }
            if (isBasedOnViewModel)
                await ViewModelLocator.Current.MainViewModel.FreqCounterViewModel.DisplayPlots(true);//this will count everything
            else
                await _StoringMeasurementsService.DisplayMeasurementByModelAsync(value.Model);

            ViewModelLocator.Current.MainViewModel.IsDoingSomethingImportant = false;
        }



        internal void AddVms(List<MeasurementViewModel> vms)
        {
            vms.ForEach(x => MeasurementsVms.Add(x));
            MeasurementsVms.Sort(x => x.DateTime);
        }

        internal async Task RemoveVm(MeasurementViewModel measurementViewModel)
        {
            if (await DialogService.DeleteMeasurementDialog())
            {
                await _DataService.DeleteStoredViewModel(measurementViewModel);
                MeasurementsVms.Remove(measurementViewModel);
                await SelectedMeasurementVmSet(null);
            }
        }

        public async Task DeleteAllMeasurements()
        {

            if (await DialogService.DeleteMeasurementDialog("Delete All Measurements?"))
            {
                await _DataService.DeleteAllMeasurementsForCurrentVideoFile();
                MeasurementsVms.Clear();
                await SelectedMeasurementVmSet(null);

            }
        }


        //this is called when freqprogress step is changed -> new model is created..
        public async Task SaveSelectedMeasurement()
        {
            if (SelectedMeasurementVm != null)
         await       _StoringMeasurementsService.GetModelFromVmAndSaveItToFile(SelectedMeasurementVm);
        }

        //internal async Task SelectAndDisplayLastInAny()
        //{
        //    var lastmea = MeasurementsVms.OrderBy(x => x.DateTime).LastOrDefault();
        //    if (lastmea != null)
        //        await SelectedMeasurementVmSet(lastmea);
        //}

        
        internal async Task SelectAndDisplayLastForVideo(string  falToken)
        {
            var lastMea = MeasurementsVms.Where(x => x.VideoFileModel.FalToken == falToken).OrderBy(x => x.DateTime).LastOrDefault();
            if (lastMea != null)
                await SelectedMeasurementVmSet(lastMea);
        }

        internal async Task AddAndSelectVm(MeasurementViewModel vm)
        {
            MeasurementsVms.Add(vm);
            MeasurementsVms.Sort(x => x.DateTime);
            await SelectedMeasurementVmSet(vm, true);

        }
    }

    public class MeasurementViewModel : ViewModelBase
    {


        //public string FalTokenOfVideo { get; set; }

        public VideoFileModel VideoFileModel { get; set; }
        public bool IsVectorDataLoaded { get; set; }

        public MeasurementViewModel(MeasurementModel model,VideoFileModel vfm)
        {
            DateTime = model.DateTime;
            Id = model.Id;
            Model = model;
            Name = model.Name;
            VideoFileModel = vfm;   
            //_dataservice = ds;
        }
        //private DataService _dataservice;
        private string _Name;

        public string Name
        {
            get => _Name;
            set
            {
                Set(ref _Name, value);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    NameToDisplay = value;
                }
                else
                {
                    NameToDisplay = DateTime.ToString(Defaults.DateTimeFormatForMeasurements);
                }
            }
        }
        /// <summary>
        /// this is folder only for this paritcular measurement. Thus deleting it, deletes this measurement
        /// </summary>
        public StorageFolder FolderForMeasurement { get; set; }

        public MeasurementModel Model { get; set; }

        private string _NameToDisplay;

        public string NameToDisplay
        {
            get => _NameToDisplay;
            set => Set(ref _NameToDisplay, value);
        }

        private DateTime _DateTime;

        public DateTime DateTime
        {
            get => _DateTime;
            set
            {
                _DateTime = value;
                Set(ref _DateTime, value);
            }
        }



        private Guid _Id;

        public Guid Id
        {
            get => _Id;
            set => Set(ref _Id, value);
        }

        public async Task DeleteMe()
        {
            await ViewModelLocator.Current.PastMeasurementsViewModel.RemoveVm(this);
        }

        public async Task EditName()
        {
            var newName = await DialogService.EditNameDialog(Name);
            if (newName != null && Name != newName)
            {
                Name = newName;
                this.Model.Name = newName;
                await DataService.SaveMeasurementResults(Model, FolderForMeasurement,true);
            }
        }

    }

}
