using System;
using TremAn3.ViewModels;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;
using Microsoft.Toolkit.Uwp.UI;

namespace TremAn3.Views
{
    public sealed partial class MainPage : Page
    {

        private MainViewModel ViewModel => ViewModelLocator.Current.MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModelLocator.Current.NoificationViewModel.NotificationHandler += ViewModel_NotificationHandler;
            ViewModel.SetTitle =  s=> ApplicationViewExtensions.SetTitle(this, s);//todo: move this to xaml back (broken in winui2.7)
        }

        private void ViewModel_NotificationHandler(string message)
        {
            Notif.Show(message,2000);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MediaPlayback.WhenNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            MediaPlayback.WhenNavigatedFrom();
        }
    }

}
