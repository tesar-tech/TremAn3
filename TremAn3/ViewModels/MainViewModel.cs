using System;

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
            await grabber.GrabGrayFrameInPositionAsync();
        }


        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = ViewModelLocator.Current.MediaPlayerViewModel;
    }
}
