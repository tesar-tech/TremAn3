using System;

using GalaSoft.MvvmLight.Ioc;

using TremAn3.Services;
using TremAn3.Views;

namespace TremAn3.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        private ViewModelLocator()
        {
            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register(() => new MediaPlayerViewModel());
            SimpleIoc.Default.Register(() => new NotificationViewModel());
            Register<MainViewModel, MainPage>();
            //Register<MediaPlayerViewModel, MediaPlayerPage>();
            Register<SettingsViewModel, SettingsPage>();
            Register<VideoInfoViewModel, VideoInfoPage>();
            Register<SchemeActivationSampleViewModel, SchemeActivationSamplePage>();
        }

        public SchemeActivationSampleViewModel SchemeActivationSampleViewModel => SimpleIoc.Default.GetInstance<SchemeActivationSampleViewModel>();

        public SettingsViewModel SettingsViewModel => SimpleIoc.Default.GetInstance<SettingsViewModel>();
        public VideoInfoViewModel VideoInfoViewModel => SimpleIoc.Default.GetInstance<VideoInfoViewModel>();

        // A Guid is generated as a unique key for each instance as reusing the same VM instance in multiple MediaPlayerElement instances can cause playback errors
        public MediaPlayerViewModel MediaPlayerViewModel => SimpleIoc.Default.GetInstance<MediaPlayerViewModel>(Guid.NewGuid().ToString());

        public NotificationViewModel NoificationViewModel => SimpleIoc.Default.GetInstance<NotificationViewModel>();
        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        public void Register<VM, V>()
            where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
