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

namespace TremAn3.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }



        public async void OpenVideo_ButtonClickAsync()
        {
             var file = await DataService.OpenFileDialogueAsync();
            MediaPlayerViewModel.ChangeSource(file);
        }
        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = new MediaPlayerViewModel();
        public DataService DataService { get; set; } = new DataService();
    }
}
