using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using TremAn3.ViewModels;
using TremAn3.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Media;
using TremAn3.Core;
using Microsoft.Toolkit.Uwp.UI.Converters;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Threading.Tasks;
using FFmpegInterop;
using TremAn3.Helpers;

namespace TremAn3.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            FreqCounterViewModel   = new FreqCounterViewModel(this);
        }
        //public event EventHandler NotificationHandler;

        public event Action<string> NotificationHandler;

        public async void LoadedAsync()
        {
            //await MediaPlayerViewModel.SetDefaultSourceAsync();
            //FreqCounterViewModel.Maximum = MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds;
            //FreqCounterViewModel.Minrange = 0d;
            IsFreqCounterOpen = false;// more info in IsFreqCounterOpen comment
        }



        public async void OpenVideo_ButtonClickAsync()
        {
            var file = await DataService.OpenFileDialogueAsync();
            await MediaPlayerViewModel.ChangeSourceAsync(file);
            FreqCounterViewModel.ResetResultDisplay();
            FreqCounterViewModel.Maximum = MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds;
        }
        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = new MediaPlayerViewModel();
        public DataService DataService { get; set; } = new DataService();
        public async Task CountFreqAsync()
        {
            if (MediaPlayerViewModel.Source == null)
            {
                NotificationHandler.Invoke("Load video first!");
                return;
            }
            FreqCounterViewModel.IsComputationInProgress = true;
            FreqCounterViewModel.ResetResultDisplay();
            
            FramesGrabber grabber = await FramesGrabber.CtorAsync(MediaPlayerViewModel.CurrentStorageFile,MediaPlayerViewModel.VideoPropsViewModel,
                    FreqCounterViewModel.PercentageOfResolution, TimeSpan.FromSeconds( FreqCounterViewModel.Minrange), TimeSpan.FromSeconds(FreqCounterViewModel.Maxrange));
            var frameRate = MediaPlayerViewModel.VideoPropsViewModel.FrameRate;
            comAlg = new CenterOfMotionAlgorithm(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight, frameRate);

            int counter = 0;
            double grabbingtime = 0;
            double getComTime = 0;
            Stopwatch sw = new Stopwatch();
            while (true)
            {
             
                sw.Restart();
                //Debug.WriteLine(counter++);
                if (comAlg.Frame2 == null)//on the beginning 
                    comAlg.Frame1 = await grabber.GrabARGBFrameInCurrentIndexAsync();
                else
                    comAlg.Frame1 = comAlg.Frame2;//because of diff

                comAlg.Frame2 = await grabber.GrabARGBFrameInCurrentIndexAsync();
                grabbingtime += sw.ElapsedMilliseconds;
                sw.Restart();
                if (comAlg.Frame2 == null)
                    break;
                comAlg.GetComFromCurrentARGBFrames();
                FreqCounterViewModel.ProgressPercentage = grabber.ProgressPercentage;
                getComTime += sw.ElapsedMilliseconds;
                // frame grabber is bad on small videos - no idea why
            }

           
            FreqCounterViewModel.VideoMainFreq= comAlg.GetMainFreqAndFillPsdDataFromComLists();

            FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.PSDAvg, comAlg.PsdAvgData);

            var xComs = comAlg.ListComXNoAvg.Select((x, i) => ((double)i, x)).ToList();
            FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.XCoM, xComs);

            var YComs = comAlg.ListComYNoAvg.Select((x, i) => ((double)i, x)).ToList();
            FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.YCoM, xComs);

            FreqCounterViewModel.IsComputationInProgress = false;
        }

        CenterOfMotionAlgorithm comAlg;



        public async Task ExportCoMsAsync()
        {
            if (comAlg == null)
            {
                NotificationHandler.Invoke("Nothing to export");
                return;
            }
            var separators = (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator);
            var str = CsvBuilder.GetCsvFromTwoLists(comAlg.ListComXNoAvg, comAlg.ListComYNoAvg, separators, "frame", "CoMX", "CoMY");
            var name = $"{MediaPlayerViewModel.VideoPropsViewModel.DisplayName}_CoMs";
            var status = await CsvExport.ExportStringAsCsvAsync(str, name);
            NotifBasedOnStatus(status, name);
        }

        public async Task ExportPsdAsync()
        {
            if (comAlg == null)
            {
                NotificationHandler.Invoke("Nothing to export");
                return;
            }
            var separators = (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator);
            var str = CsvBuilder.GetCsvFromData(comAlg.PsdAvgData, separators, "frequency", "PSD");
            var name = $"{MediaPlayerViewModel.VideoPropsViewModel.DisplayName}_PSD";
            var status = await CsvExport.ExportStringAsCsvAsync(str, name);
            NotifBasedOnStatus(status, name);


        }


        private void NotifBasedOnStatus(CsvExport.CsvExportStatus status, string filename)
        {
            switch (status)
            {
                case CsvExport.CsvExportStatus.Completed:
                    NotificationHandler.Invoke($"File ({filename}) was saved");
                    break;
                case CsvExport.CsvExportStatus.NotCompleted:
                    NotificationHandler.Invoke("File couldn't be saved");
                    break;
                default:
                    break;
            }
        }
  

   

        private bool _IsFreqCounterOpen = true;//this is necessary workaround for splitView not showinx oxyplot. Freq counter is closed after page is loaded. 

        public bool IsFreqCounterOpen
        {
            get => _IsFreqCounterOpen;
            set => Set(ref _IsFreqCounterOpen, value);
        }

        public FreqCounterViewModel FreqCounterViewModel { get; set; }
        //public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;


    }
}
