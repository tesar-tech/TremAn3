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

namespace TremAn3.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        //public MainViewModel()
        //{

        //}


        public async void LoadedAsync()
        {
            await MediaPlayerViewModel.SetDefaultSourceAsync();
            FreqCounterViewModel.Maximum = MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds;
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
        public async void GetFrameClickAsync()
        {
            FreqCounterViewModel.IsComputationInProgress = true;
            FreqCounterViewModel.ResetResultDisplay();
            
            FramesGrabber grabber = await FramesGrabber.CtorAsync(MediaPlayerViewModel.CurrentStorageFile,MediaPlayerViewModel.VideoPropsViewModel,FreqCounterViewModel.PercentageOfResolution);
            var frameRate = MediaPlayerViewModel.VideoPropsViewModel.FrameRate;
            CenterOfMotionAlgorithm comAlg = new CenterOfMotionAlgorithm(grabber.DecodedPixelWidth, grabber.DecodedPixelHeight, frameRate);

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
                ProgressPercentage = grabber.ProgressPercentage;
                getComTime += sw.ElapsedMilliseconds;
                // frame grabber is bad on small videos - no idea why
            }

            //comAlg.listComX
            
            var window = 200;
            var avgX =  comAlg.listComX.Average();

            var withoutAvgLisX = comAlg.listComX.Select(x => x - avgX).ToList();
            List<double> vysX = Fft.ComputeFftDuringSignal(frameRate, withoutAvgLisX, window, 5, false);

            var avgY = comAlg.listComY.Average();
            var withoutAvgLisY = comAlg.listComY.Select(x => x - avgY).ToList();
            List<double> vysY = Fft.ComputeFftDuringSignal(frameRate, withoutAvgLisY, window, 5, false);


            List<(double xx, double yy)> datP = new List<(double xx, double yy)>();
            double vx = 0;

            var vys = vysX.Zip(vysY, (v1, v2) => (v1 + v2) / 2).ToList();
            var vysTime = MediaPlayerViewModel.VideoPropsViewModel.Duration.TotalSeconds / vys.Count;
            foreach (var v in vys)
            {
                datP.Add((vx,v));
                vx += vysTime;
            }

           FreqCounterViewModel.VideoMainFreq = comAlg.GetMainFreqFromComLists();
            FreqCounterViewModel.UpdatePlotWithNewVals(datP);
            FreqCounterViewModel.IsComputationInProgress = false;
        }

        private double _ProgressPercentage;

        public double ProgressPercentage
        {
            get => _ProgressPercentage;
            set => Set(ref _ProgressPercentage, value);
        }


        
        Random random = new Random();
        //public void GenRanNum()
        //{ 
        //    int num = random.Next(100);
        //    //string str =num.ToString();
        //    VideoMainFreq = num;
        //}

        private bool _IsFreqCounterOpen = true;//this is necessary workaround for splitView not showinx oxyplot. Freq counter is closed after page is loaded. 

        public bool IsFreqCounterOpen
        {
            get => _IsFreqCounterOpen;
            set => Set(ref _IsFreqCounterOpen, value);
        }

        public FreqCounterViewModel FreqCounterViewModel { get; set; } = new FreqCounterViewModel();
        //public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;


      

    }
}
