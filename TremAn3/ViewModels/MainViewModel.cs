using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using TremAn3.Services;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using TremAn3.Core;

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
            grabber.batchSize = 1000;

            await grabber.ChangeStorageFileAsync(file);
            (int width, int height) = grabber.GetWidthAndHeight();
            CenterOfMotionAlgorithm comAlg = new CenterOfMotionAlgorithm(width, height,grabber.FrameRate);

            int counter = 0;
            while (true)
            {
                Debug.WriteLine(counter++);
                if (comAlg.Frame2 ==null)//on the beginning 
                    comAlg.Frame1 = await grabber.GrabGrayFrameInCurrentIndexAsync();
                else
                     comAlg.Frame1 = comAlg.Frame2;//because of diff

                comAlg.Frame2 = await grabber.GrabGrayFrameInCurrentIndexAsync();

                if (comAlg.Frame2 == null)
                    break;
               comAlg.GetComFromCurrentFrames();
                
                // frame grabber is bad on small videos - no idea why
            }

         

            VideoMainFreq = comAlg.GetMainFreqFromComLists();
        }

        private double _VideoMainFreq;

        public double VideoMainFreq
        {
            get => _VideoMainFreq;
            set => Set(ref _VideoMainFreq, value);
        }



        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;
    }
}
