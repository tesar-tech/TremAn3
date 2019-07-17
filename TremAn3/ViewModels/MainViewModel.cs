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

namespace TremAn3.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }



        public async void OpenVideo_ButtonClickAsync(object sender, RoutedEventArgs e)
        {
            //call service for openFileDialog
            //recieve video video file (StorageFile)
            // create method MediaPlayerViewModel.ChangeSource(StorageFile)
            // call for MediaSource.CreateFromStorageFile
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            { 
                MediaPlayerViewModel.Source = MediaSource.CreateFromStorageFile(file);
            }
        }
        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = new MediaPlayerViewModel();
    }
}
