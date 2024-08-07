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

namespace TremAn3.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(DataService dataService, MeasurementsService sms)
    {
        _DataService = dataService;
        _StoringMeasurementsService = sms;
        ExportService = new ExportService();
        //FreqCounterViewModel = new FreqCounterViewModel(this);
    }
    //public event EventHandler NotificationHandler;

    private MeasurementsService _StoringMeasurementsService;

    public async void LoadedAsync()
    {
        //#if DEBUG
        //            StorageFile videoFile = (StorageFile)(await KnownFolders.PicturesLibrary.TryGetItemAsync("hand.mp4"));
        //            await OpenStorageFile(videoFile);
        //#endif
        if (ViewModelLocator.Current.SettingsViewModel.IsLoadRecentVideoOnAppStart)
        {
            var videoFile = await _DataService.GetLastOpenedFile();
            await OpenStorageFile(videoFile);
            ViewModelLocator.Current.DrawingRectanglesViewModel.RefreshSizeProportion();

        }
    }

    public LoadingContentViewModel LoadingContentViewModel => ViewModelLocator.Current.LoadingContentViewModel;

    private bool _IsVideoFileLoaded;

    /// <summary>
    /// when the video file is not presented, but measurement are. This hides video controls and computation button
    /// </summary>
    public bool IsVideoFileLoaded
    {
        get => _IsVideoFileLoaded;
        set => Set(ref _IsVideoFileLoaded, value);
    }


    public async void OpenVideo_ButtonClickAsync()
    {
        var file = await _DataService.OpenFileDialogueAsync();
        await OpenStorageFile(file);
    }

    public async Task OpenFileByFalToken(string falToken, bool isMeasurementAlreadyDisplayed)
    {
        var videoFile = await _DataService.GetFileByFalToken(falToken);
        if (videoFile != null)
            await OpenStorageFile(videoFile, isMeasurementAlreadyDisplayed);
        else if (isMeasurementAlreadyDisplayed)
            MediaPlayerViewModel.ChangeSourceToNothing();
    }

    private async Task OpenStorageFile(StorageFile file, bool isMeasurementAlreadyDisplayed = false)
    {
        try
        {
            await ViewModelLocator.Current.MainViewModel.PastMeasurementsViewModel.SelectedMeasurementVmSet(null);
            if (file == null) return;
            await MediaPlayerViewModel.ChangeSourceAsync(file);
            if (!isMeasurementAlreadyDisplayed)
            {
                FreqCounterViewModel.ResetFreqCounter();
                IsFreqCounterOpen = true;
            }
            (MediaPlayerViewModel.CurrentMruToken, MediaPlayerViewModel.CurrentFalToken) = _DataService.SaveOpenedFileToMruAndFal(file);
        }
        catch (Exception ex)
        {
            ViewModelLocator.Current.NoificationViewModel.SimpleNotification($"Something went wrong opening video file ({file.Name}). Error message: {ex.Message}");
        }


        if (!PastMeasurementsViewModel.IsAllMeasurementsLoaded)
        {
            var allPastMeasurements = await _DataService.GetAllPastMeasurements();
            PastMeasurementsViewModel.AddVms(allPastMeasurements);
            PastMeasurementsViewModel.IsAllMeasurementsLoaded = true;

        }
        if (!isMeasurementAlreadyDisplayed)
            await PastMeasurementsViewModel.SelectAndDisplayLastForVideo(MediaPlayerViewModel.CurrentFalToken);
    }
    public PastMeasurementsViewModel PastMeasurementsViewModel => ViewModelLocator.Current.PastMeasurementsViewModel;

    public MediaPlayerViewModel MediaPlayerViewModel { get => ViewModelLocator.Current.MediaPlayerViewModel; }

    private DataService _DataService;
    CancellationTokenSource source;

    public async Task CountFreqAsync()
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
        x => x.InitializeCoM(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight, frameRate, FreqCounterViewModel.PercentageOfResolution, x.Color));
        var comAlgs = rois.Select(x => x.ComputationViewModel.Algorithm);

        Stopwatch sw = new Stopwatch();

        sw.Start();

        source = new CancellationTokenSource();
        await Computation(grabber, comAlgs, source);//this modifies comAlgs that are part of FreqCounterVm
        //Coherence coherenceBetween2Windows = new Coherence();



        if (!source.IsCancellationRequested)
        {
            Debug.WriteLine(sw.ElapsedMilliseconds);
            MeasurementModel measurementModel = new(comAlgs)//comalgs are not computed
            {
                //Coherence = CurrentResultsViewModel.CoherenceResult,
                Minrange = MediaPlayerViewModel.FreqCounterViewModel.Minrange,
                Maxrange = MediaPlayerViewModel.FreqCounterViewModel.Maxrange,
                PositionSeconds = MediaPlayerViewModel.MediaControllingViewModel.PositionSeconds,
                FreqProgressSegmnetSize = FreqCounterViewModel.FreqProgressViewModel.SegmnetSize,
                FreqProgressStep = FreqCounterViewModel.FreqProgressViewModel.Step,
            };

            VideoFileModel vfm = new(MediaPlayerViewModel.VideoPropsViewModel, MediaPlayerViewModel.CurrentFalToken);
            MeasurementViewModel vm = new(measurementModel, vfm);
            vm.IsVectorDataLoaded = true;
            await PastMeasurementsViewModel.AddAndSelectVm(vm);//this will display and compute plots
            await _StoringMeasurementsService.GetModelFromVmAndSaveItToFile(vm);

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
            else//creating new list every time, probably not best for performance
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




    public void NotifyBasedOnStatus(CsvExport.CsvExportStatus status, string filename)
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
        set {
            if (Set(ref _Title, value)) SetTitle?.Invoke(value);
        }
    }
    public Action<string> SetTitle;
    public void RefreshTitle(string newTitle = "")
    {
        if (newTitle == "")
            Title = MediaPlayerViewModel.VideoPropsViewModel.ToString();
        else Title = newTitle;
    }

    public FreqCounterViewModel FreqCounterViewModel { get => ViewModelLocator.Current.FreqCounterViewModel; }

    private ICommand _getStorageItemsCommand;
    public ICommand GetStorageItemsCommand => _getStorageItemsCommand ?? (_getStorageItemsCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnGetStorageItemAsync));
    public ExportService ExportService;

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
