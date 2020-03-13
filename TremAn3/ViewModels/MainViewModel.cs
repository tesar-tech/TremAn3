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
using System.Collections.ObjectModel;

namespace TremAn3.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            FreqCounterViewModel = new FreqCounterViewModel(this);
        }
        //public event EventHandler NotificationHandler;


        public async void LoadedAsync()
        {
#if DEBUG
            var videoFile = await KnownFolders.PicturesLibrary.GetFileAsync("hand.mp4");
            await MediaPlayerViewModel.ChangeSourceAsync(videoFile);
            FreqCounterViewModel.ResetFreqCounter();
            IsFreqCounterOpen = true;
#endif

        }



        public async void OpenVideo_ButtonClickAsync()
        {
            var file = await DataService.OpenFileDialogueAsync();
            if (file != null)
            {
                await MediaPlayerViewModel.ChangeSourceAsync(file);
                FreqCounterViewModel.ResetFreqCounter();
                IsFreqCounterOpen = true;

            }
        }
        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = new MediaPlayerViewModel();
        public DataService DataService { get; set; } = new DataService();
        //CenterOfMotionAlgorithm comAlg;
        //List<CenterOfMotionAlgorithm> comAlgs = new List<CenterOfMotionAlgorithm>();
        public async void CountFreqAsync()
        {
            if (MediaPlayerViewModel.Source == null)
            {
                ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Load video first!");
                return;
            }
            FreqCounterViewModel.IsComputationInProgress = true;
            FreqCounterViewModel.ResetResultDisplay();

            //SelectionRectangle rect = null;//FreqCounterViewModel.SelectionRectangleViewModel.GetModel(FreqCounterViewModel.PercentageOfResolution);

            FramesGrabber grabber = await FramesGrabber.CtorAsync(MediaPlayerViewModel.CurrentStorageFile, MediaPlayerViewModel.VideoPropsViewModel,
                    FreqCounterViewModel.PercentageOfResolution, TimeSpan.FromSeconds(FreqCounterViewModel.Minrange), TimeSpan.FromSeconds(FreqCounterViewModel.Maxrange));
            var frameRate = MediaPlayerViewModel.VideoPropsViewModel.FrameRate;

            var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels;
            if (rois.Count == 0)
                ViewModelLocator.Current.DrawingRectanglesViewModel.AddMaxRoi();

            rois.ToList().ForEach(
                x => x.InitializeCoM(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight, frameRate, FreqCounterViewModel.PercentageOfResolution));
            var comAlgs = rois.Select(x => x.ComputationViewModel.Algorithm);


            //comAlg = new CenterOfMotionAlgorithm(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight,frameRate,rect);

            //int counter = 0;
            double grabbingtime = 0;
            double getComTime = 0;
            Stopwatch sw = new Stopwatch();
            List<byte> frame1 = null;
            List<byte> frame2 = null;
            while (true)
            {


                if (frame1 == null)//on the beginning 
                    frame1 = new List<byte>((await grabber.GrabARGBFrameInCurrentIndexAsync()).data);
                else
                    frame1 = frame2;//because of diff

                var frameAndBool = await grabber.GrabARGBFrameInCurrentIndexAsync();
                if (frameAndBool.isData)
                    frame2 = new List<byte>(frameAndBool.data);
                else
                    break;
                foreach (var comAlg in comAlgs)
                {

                    //sw.Restart();
                    //Debug.WriteLine(counter++);
                    //grabbingtime += sw.ElapsedMilliseconds;
                    //sw.Restart();
                    comAlg.Frame1 = frame1;
                    comAlg.Frame2 = frame2;

                    comAlg.GetComFromCurrentARGBFrames();
                    comAlg.Results.FrameTimes.Add(grabber.TimeOfFrameOnCurrentIndex);
                }

                FreqCounterViewModel.ProgressPercentage = grabber.ProgressPercentage;
                getComTime += sw.ElapsedMilliseconds;
                // frame grabber is bad on small videos - no idea why
            }
            FreqCounterViewModel.DisplayPlots();

            //FreqCounterViewModel.VideoMainFreq = comAlgs[0].GetMainFreqAndFillPsdDataFromComLists();

            ////psd
            //FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.PSDAvg, comAlgs[0].PsdAvgData);

            ////x
            //var xComs = comAlgs[0].ListComXNoAvg.Select((x, i) => ((double)i, x)).ToList();
            //FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.XCoM, xComs);

            ////y
            //var yComs = comAlgs[0].ListComYNoAvg.Select((x, i) => ((double)i, x)).ToList();
            //FreqCounterViewModel.UpdatePlotsWithNewVals(FreqCounterViewModel.PlotType.YCoM, yComs);

            FreqCounterViewModel.IsComputationInProgress = false;
        }




        public async void ExportComXAsync() => await ExportToCsvAsync("comX");
        public async void ExportComYAsync() => await ExportToCsvAsync("comY");
        public async void ExportPsdAsync() => await ExportToCsvAsync("psd");

        public async Task ExportToCsvAsync( string type)
        {
            var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels;
            if (rois.Count == 0 || !rois[0].ComputationViewModel.HasResult)
            {
                ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Nothing to export");
                return;
            }

            var separators = (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator);

            List<double> one_x = null;
            if(type=="comX" || type =="comY")
            one_x = rois[0].ComputationViewModel.Algorithm.Results.FrameTimes.Select(x=>x.TotalSeconds).ToList();
            else if(type=="psd")
                one_x = rois[0].ComputationViewModel.Algorithm.Results.PsdAvgData.Select(x=>x.x_freq).ToList();


            List<List<double>> multiple_ys = null;

            if (type == "comX")
                multiple_ys = rois.Select(x=> x.ComputationViewModel.Algorithm.Results.ListComXNoAvg).ToList();
            else if (type == "comY")
                multiple_ys = rois.Select(x=> x.ComputationViewModel.Algorithm.Results.ListComYNoAvg).ToList();
            else if (type == "psd")
                multiple_ys = rois.Select(x=>x.ComputationViewModel.Algorithm.Results.PsdAvgData.Select(y => y.y_power).ToList()).ToList();

            string xHeader = type == "psd" ? "freq [Hz]" : "time[s]";
            string yHeader = type == "psd" ? "PSD" : type=="comX"? "CoMX" : "CoMY";

            var str = CsvBuilder.GetCvsFromOneX_MultipleY(xs:one_x, multiple_ys: multiple_ys, separators,headers:rois.Select(x=> $"{yHeader}__{x.ToString()}").Prepend(xHeader));

            //var str = CsvBuilder.GetCsvFromTwoLists(comAlg.ListComXNoAvg, comAlg.ListComYNoAvg, separators, "frame", "CoMX", "CoMY");
            var name = $"{MediaPlayerViewModel.VideoPropsViewModel.DisplayName}_{type}";
            var (status, newName) = await CsvExport.ExportStringAsCsvAsync(str, name);
            NotifBasedOnStatus(status, newName);
        }


        private void NotifBasedOnStatus(CsvExport.CsvExportStatus status, string filename)
        {
            switch (status)
            {
                case CsvExport.CsvExportStatus.Completed:
                    ViewModelLocator.Current.NoificationViewModel.SimpleNotification($"File ({filename}) was saved");
                    break;
                case CsvExport.CsvExportStatus.NotCompleted:
                    ViewModelLocator.Current.NoificationViewModel.SimpleNotification("File couldn't be saved");
                    break;
                default:
                    break;
            }
        }




        private bool _IsFreqCounterOpen = false;//this is necessary workaround for splitView not showinx oxyplot. Freq counter is closed after page is loaded. 

        public bool IsFreqCounterOpen
        {
            get => _IsFreqCounterOpen;
            set => Set(ref _IsFreqCounterOpen, value);
        }

        public FreqCounterViewModel FreqCounterViewModel { get; set; }
        //public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;


    }
}
