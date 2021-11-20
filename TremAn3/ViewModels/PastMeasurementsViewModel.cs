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
using TremAn3.Helpers;

namespace TremAn3.ViewModels
{
    public class PastMeasurementsViewModel : ViewModelBase
    {

        public PastMeasurementsViewModel(StoringMeasurementsService sms, DataService ds)
        {
            _StoringMeasurementsService = sms;
            _DataService = ds;
        }

        private StoringMeasurementsService _StoringMeasurementsService;
        private DataService _DataService;


        //private bool _IsSaveMeasurement = LocalSettings.Read(true, nameof(IsSaveMeasurement));

        //public bool IsSaveMeasurement
        //{
        //    get => _IsSaveMeasurement;
        //    set
        //    {
        //        if (Set(ref _IsSaveMeasurement, value))
        //            LocalSettings.Write(value);
        //    }
        //}


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
        internal void RoiIsNotSameAsResult()
        {
            SelectedMeasurementVm = null;
        }

        public ObservableCollection<MeasurementViewModel> MeasurementsVms
        {
            get => _MeasurementsVms;
            set => Set(ref _MeasurementsVms, value);
        }

        private MeasurementViewModel _SelectedMeasurementVm;

        public MeasurementViewModel SelectedMeasurementVm
        {
            get => _SelectedMeasurementVm;
            set
            {

                if (!Set(ref _SelectedMeasurementVm, value)) return;

                if (value == null)
                {
                    //ViewModelLocator.Current.FreqCounterViewModel.ResetResultDisplay();
                    return;
                }
                //testing onluy
                value.Model.FrameRate = 30;
                //testing onluy
                _StoringMeasurementsService.DisplayMeasurementByModel(value.Model);

            }
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
                SelectedMeasurementVm = null;
            }
        }

        public async Task DeleteAllMeasurements()
        {

            if (await DialogService.DeleteMeasurementDialog("Delete All Measurements?"))
            {
                await _DataService.DeleteAllMeasurementsForCurrentVideoFile();
                MeasurementsVms.Clear();
                SelectedMeasurementVm = null;
            }
        }




        internal void SelectAndDisplayLastInAny()
        {
            var lastmea = MeasurementsVms.OrderBy(x => x.DateTime).LastOrDefault();
            if (lastmea != null)
                SelectedMeasurementVm = lastmea;

        }

        internal void AddAndSelectVm(MeasurementViewModel vm)
        {
            MeasurementsVms.Add(vm);
            MeasurementsVms.Sort(x => x.DateTime);
            SelectedMeasurementVm = vm;
        }
    }

    public class MeasurementViewModel : ViewModelBase
    {

        public MeasurementViewModel(MeasurementModel model)
        {
            DateTime = model.DateTime;
            Id = model.Id;
            Model = model;
            Name = model.Name;
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
                await DataService.SaveMeasurementResults(Model,FolderForMeasurement);
            }
        }


    }

}
