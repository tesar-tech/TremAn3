using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using TremAn3.Services;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using FFmpegInterop;

namespace TremAn3.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }

        public void OpenVideo_ButtonClick()
        {
            //call service for openFileDialog
            //recieve video video file (StorageFile)
            // create method MediaPlayerViewModel.ChangeSource(StorageFile)
            // call for MediaSource.CreateFromStorageFile
        }

        private ImageSource _CurrentFrameSource;

        public ImageSource CurrentFrameSource
        {
            get => _CurrentFrameSource;
            set => Set(ref _CurrentFrameSource, value);
        }


        public async void GetFrameClickAsync()
        {

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/beru.wmv"));
            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/beru_small.avi"));

            FramesGrabber grabber = new FramesGrabber();
            await grabber.ChangeStorageFileAsync(file);
            (int width, int height) = grabber.GetWidthAndHeight();
            var vecOfx = Enumerable.Repeat(Enumerable.Range(0, width), height).SelectMany(x => x);
            var vecOfy = Enumerable.Range(0, height).Select(x => Enumerable.Repeat(x, width)).SelectMany(x => x);
            bool stillSomeFrame = true;
            byte[] frame2 = new byte[] { };
            byte[] frame1 = new byte[] { };
            double[] diff = new double[] { };
            grabber.batchSize = 1000;
            Stopwatch s = new Stopwatch();
            s.Start();
            var listComX = new List<double>();
            var listComY = new List<double>();
            int counter = 0;
            while (frame2 != null)
            {
                Debug.WriteLine(counter++);
                if (frame2.Length == 0)//na zacatku
                    frame1 = await grabber.GrabGrayFrameInCurrentIndexAsync();
                else
                    frame1 = frame2;//because of diff
                string ss = "";
                //foreach (var i in frame1)
                //{
                //     ss = $"{ss},{i}";
                //}
                frame2 = await grabber.GrabGrayFrameInCurrentIndexAsync();

                if (frame2 != null)
                    diff = frame2.Zip(frame1, (f2, f1) => (double)f2 - f1).ToArray();
                //normalize frames
                var max = diff.Max();
                var min = diff.Min();
                if (max == 0 && min == 0)
                {
                    if (listComX.Count == 0)//first frame is same as second
                    {
                        listComX.Add(width / 2);//just add center of picture 
                        listComY.Add(height / 2);
                    }
                    else
                    {
                        listComX.Add(listComX.Last());//just copy previous value
                        listComY.Add(listComY.Last());
                    }
                    continue;//jump to next frame
                }
                var diffNorm = diff.Select(x => (x - min) / (max-min));
                //now "pixels" are in range from  0 to 1
                var mean = diffNorm.Average();

                var diffNormMultX = diffNorm.Zip(vecOfx, (di, vx) => di * vx);
                var comX = diffNormMultX.Average() / mean;
                listComX.Add(comX);

                var diffNormMultY = diffNorm.Zip(vecOfy, (di, vy) => di * vy);
                var comY = diffNormMultX.Average() / mean;
                listComY.Add(comY);
                // this is state where algorithm works - compering to matlab
                // but is slow and needs some clean up
                // frame grabber is bad on small videos - no idea why

            }

            string ssd = "";
            foreach (var i in listComX)
            {
                Debug.WriteLine(i);
                ssd = $"{ssd},{i}";
            }

            Debug.WriteLine(s.ElapsedMilliseconds);
        }


        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;
    }
}
