﻿using System;
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
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using TremAn3.Models;

namespace TremAn3.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(DataService  dataService)
        {
            _DataService = dataService;
            //FreqCounterViewModel = new FreqCounterViewModel(this);
        }
        //public event EventHandler NotificationHandler;


        public async void LoadedAsync()
        {
//#if DEBUG
//            StorageFile videoFile = (StorageFile)(await KnownFolders.PicturesLibrary.TryGetItemAsync("hand.mp4"));
//            await OpenStorageFile(videoFile);
//#endif
            if (ViewModelLocator.Current.SettingsViewModel.IsLoadRecentVideoOnAppStart)
            {
                var videoFile =await _DataService.GetLastOpenedFile();
                await OpenStorageFile(videoFile);
            }

        }



        public async void OpenVideo_ButtonClickAsync()
        {
            var file = await _DataService.OpenFileDialogueAsync();
            await OpenStorageFile(file);
        }

        private async Task OpenStorageFile(StorageFile file)
        {
            if (file != null)
            {
                await MediaPlayerViewModel.ChangeSourceAsync(file);
                FreqCounterViewModel.ResetFreqCounter();
                IsFreqCounterOpen = true;
                _DataService.SaveOpenedFileToMru(file);
            }
        }

        public MediaPlayerViewModel MediaPlayerViewModel { get => ViewModelLocator.Current.MediaPlayerViewModel; }
        private DataService _DataService;
        CancellationTokenSource source;

        bool isNotInterrupt = true;
        public async void CountFreqAsync()
        {
            if (FreqCounterViewModel.IsComputationInProgress)
            {
                source.Cancel();
                return;
            }
            if (MediaPlayerViewModel.Source == null)
            {
                ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Load video first!");
                return;
            }
            FreqCounterViewModel.IsComputationInProgress = true;
            FreqCounterViewModel.ResetResultDisplay();

            FramesGrabber grabber = await FramesGrabber.CtorAsync(MediaPlayerViewModel.CurrentStorageFile, MediaPlayerViewModel.VideoPropsViewModel,
                    FreqCounterViewModel.PercentageOfResolution, TimeSpan.FromSeconds(FreqCounterViewModel.Minrange), TimeSpan.FromSeconds(FreqCounterViewModel.Maxrange));
            var frameRate = MediaPlayerViewModel.VideoPropsViewModel.FrameRate;

            var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels;
            if (rois.Count == 0)
                ViewModelLocator.Current.DrawingRectanglesViewModel.AddMaxRoi();

            rois.ToList().ForEach(
                x => x.InitializeCoM(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight, frameRate, FreqCounterViewModel.PercentageOfResolution));
            var comAlgs = rois.Select(x => x.ComputationViewModel.Algorithm);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            source = new CancellationTokenSource();
            await Computation(grabber, comAlgs, source);//this modifies comAlgs that are part of FreqCounterVm

            if (!source.IsCancellationRequested)
            {
                Debug.WriteLine(sw.ElapsedMilliseconds);
                FreqCounterViewModel.DisplayPlots();
            }
            else
            {
                FreqCounterViewModel.ProgressPercentage = 0;
            }
            FreqCounterViewModel.IsComputationInProgress = false;

        }

        private async Task Computation(FramesGrabber grabber, IEnumerable<CenterOfMotionAlgorithm> comAlgs, CancellationTokenSource source)
        {//frame grabber shoul be an interface and this mehtod should be in Core project.
            List<byte> frame1 = null;
            List<byte> frame2 = null;
            while (true)
            {
                if (source.IsCancellationRequested)
                    return;

                if (frame1 == null)//on the beginning 
                    frame1 = new List<byte>((await grabber.GrabARGBFrameInCurrentIndexAsync()).data);
                else
                    frame1 = frame2;//because of diff

                var (data, isData) = await grabber.GrabARGBFrameInCurrentIndexAsync();
                if (isData)
                    frame2 = new List<byte>(data);
                else //creating new list every time, probably not best for performance
                    break;
                foreach (var comAlg in comAlgs)
                {
                    comAlg.Frame1 = frame1;
                    comAlg.Frame2 = frame2;

                    comAlg.GetComFromCurrentARGBFrames();
                    comAlg.Results.FrameTimes.Add(grabber.TimeOfFrameOnCurrentIndex);
                }

                FreqCounterViewModel.ProgressPercentage = grabber.ProgressPercentage;
                // frame grabber is bad on small videos - no idea why - now ain't sure about this comment
            }
        }



        public async void ExportComXAsync() => await ExportToCsvAsync("comX");
        public async void ExportComYAsync() => await ExportToCsvAsync("comY");
        public async void ExportPsdAsync() => await ExportToCsvAsync("psd");

        public async Task ExportToCsvAsync(string type)
        {
            var rois = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels.Where(x => x.IsShowInPlot && x.ComputationViewModel.HasResult).ToList();
            if (rois.Count == 0)
            {
                ViewModelLocator.Current.NoificationViewModel.SimpleNotification("Nothing to export");
                return;
            }

            var separators = (ViewModelLocator.Current.SettingsViewModel.DecimalSeparator, ViewModelLocator.Current.SettingsViewModel.CsvElementSeparator);

            List<double> one_x = null;
            if (type == "comX" || type == "comY")
                one_x = rois[0].ComputationViewModel.Algorithm.Results.FrameTimes.Select(x => x.TotalSeconds).ToList();
            else if (type == "psd")
                one_x = rois[0].ComputationViewModel.Algorithm.Results.PsdAvgData.Select(x => x.x_freq).ToList();


            List<List<double>> multiple_ys = null;

            if (type == "comX")
                multiple_ys = rois.Select(x => x.ComputationViewModel.Algorithm.Results.ListComXNoAvg).ToList();
            else if (type == "comY")
                multiple_ys = rois.Select(x => x.ComputationViewModel.Algorithm.Results.ListComYNoAvg).ToList();
            else if (type == "psd")
                multiple_ys = rois.Select(x => x.ComputationViewModel.Algorithm.Results.PsdAvgData.Select(y => y.y_power).ToList()).ToList();

            string xHeader = type == "psd" ? "freq [Hz]" : "time[s]";
            string yHeader = type == "psd" ? "PSD" : type == "comX" ? "CoMX" : "CoMY";

            var str = CsvBuilder.GetCvsFromOneX_MultipleY(xs: one_x, multiple_ys: multiple_ys, separators, headers: rois.Select(x => $"{yHeader}__{x.ToString()}").Prepend(xHeader));

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

        private bool _IsFreqCounterOpen = false;

        public bool IsFreqCounterOpen
        {
            get => _IsFreqCounterOpen;
            set => Set(ref _IsFreqCounterOpen, value);
        }
        private string _Title = "";
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        public void RefreshTitle() => Title = MediaPlayerViewModel.VideoPropsViewModel.ToString();

        public FreqCounterViewModel FreqCounterViewModel { get => ViewModelLocator.Current.FreqCounterViewModel; }

        private ICommand _getStorageItemsCommand;
        public ICommand GetStorageItemsCommand => _getStorageItemsCommand ?? (_getStorageItemsCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnGetStorageItemAsync));

        public async void OnGetStorageItemAsync(IReadOnlyList<IStorageItem> items)
        {
            foreach (var item in items)
            {
                var sf = item as StorageFile;
                if (sf != null && sf.IsFileSupported())
                {
                    await OpenStorageFile(sf);//opens first one
                    return;
                }
            }
            ViewModelLocator.Current.NoificationViewModel.SimpleNotification("File type isn't supported");

        }

  

    }
}
