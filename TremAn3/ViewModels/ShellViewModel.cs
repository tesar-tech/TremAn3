using System;
using System.Collections.Generic;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TremAn3.Helpers;
using TremAn3.Services;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TremAn3.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        private IList<KeyboardAccelerator> _keyboardAccelerators;

        private ICommand _loadedCommand;
        private ICommand _menuViewsMainCommand;
        private ICommand _menuViewsMediaPlayerCommand;
        private ICommand _menuFileVideoInfoCommand;
        private ICommand _menuFileSettingsCommand;
        private ICommand _menuFileExitCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand MenuViewsMainCommand => _menuViewsMainCommand ?? (_menuViewsMainCommand = new RelayCommand(OnMenuViewsMain));

        public ICommand MenuViewsMediaPlayerCommand => _menuViewsMediaPlayerCommand ?? (_menuViewsMediaPlayerCommand = new RelayCommand(OnMenuViewsMediaPlayer));

        public ICommand MenuFileVideoInfoCommand => _menuFileVideoInfoCommand ?? (_menuFileVideoInfoCommand = new RelayCommand(OnMenuFileVideoInfo));

        public ICommand MenuFileSettingsCommand => _menuFileSettingsCommand ?? (_menuFileSettingsCommand = new RelayCommand(OnMenuFileSettings));

        public ICommand MenuFileExitCommand => _menuFileExitCommand ?? (_menuFileExitCommand = new RelayCommand(OnMenuFileExit));

        public static NavigationServiceEx NavigationService => ViewModelLocator.Current.NavigationService;

        public ShellViewModel()
        {
        }

        public void OpenNewFile()
        {
            ViewModelLocator.Current.MainViewModel.OpenVideo_ButtonClickAsync();
        }

        public void FreqCounter()
        {
            ViewModelLocator.Current.MainViewModel.IsFreqCounterOpen = !ViewModelLocator.Current.MainViewModel.IsFreqCounterOpen;
        }

        public void Initialize(Frame shellFrame, SplitView splitView, Frame rightFrame, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            NavigationService.Frame = shellFrame;
            MenuNavigationHelper.Initialize(splitView, rightFrame);
            _keyboardAccelerators = keyboardAccelerators;
        }

        private void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
        }

        private void OnMenuViewsMain() => MenuNavigationHelper.UpdateView(typeof(MainViewModel).FullName);

        private void OnMenuViewsMediaPlayer() => MenuNavigationHelper.UpdateView(typeof(MediaPlayerViewModel).FullName);

        private void OnMenuFileVideoInfo()
        {
            ViewModelLocator.Current.VideoInfoViewModel.CurrentVideoFileProps = ViewModelLocator.Current.MainViewModel.MediaPlayerViewModel.CurrentVideoFileProps;
            MenuNavigationHelper.OpenInRightPane(typeof(Views.VideoInfoPage));
        }

        private void OnMenuFileSettings() => MenuNavigationHelper.OpenInRightPane(typeof(Views.SettingsPage));

        private void OnMenuFileExit()
        {
            Application.Current.Exit();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}
