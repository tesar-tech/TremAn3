using System;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using TremAn3.Services;
using Windows.Storage;
using Windows.UI.Xaml.Media;

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

            FramesGrabber grabber = new FramesGrabber();
            await grabber.ChangeStorageFileAsync(file);
            (int width, int height) = grabber.GetWidthAndHeight();
            var vecOfx = Enumerable.Repeat(Enumerable.Range(0, width), height).SelectMany(x => x);
            var vecOfy = Enumerable.Range(0, height).Select(x => Enumerable.Repeat(x, width)).SelectMany(x => x);
            bool stillSomeFrame = true;
            byte[] frame2 = new byte[] { };
            byte[] frame1 = new byte[] { };
            int[] diff = new int[] { };
            grabber.batchSize = 1000;
            Stopwatch s = new Stopwatch();
            s.Start();
            while (frame2 != null)
            {
                if (frame2.Length == 0)//na zacatku
                    frame1 = await grabber.GrabGrayFrameInCurrentIndexAsync();
                else
                    frame1 = frame2;//because of diff

                frame2 = await grabber.GrabGrayFrameInCurrentIndexAsync();
                if (frame2 != null)
                    diff = frame2.Zip(frame1, (f2, f1) => f2 - f1).ToArray();
                //normalize frames
                var max = diff.Max();
                var min = diff.Min();
                var diffNorm = diff.Select(x => (x - min) / max);
                //now "pixels" are in range from  0 to 1
                var mean = diffNorm.Average();

                var diffNormMultX = diffNorm.Zip(vecOfx, (di, vx) => di * vx);
                var comX = diffNormMultX.Average() / mean;


                var diffNormMultY = diffNorm.Zip(vecOfy, (di, vy) => di * vy);
                var comY = diffNormMultX.Average() / mean;



            }
            Debug.WriteLine(s.ElapsedMilliseconds);
        }


        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;
    }
}
