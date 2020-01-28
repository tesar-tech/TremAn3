using System;
using TremAn3.ViewModels;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;

namespace TremAn3.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
            ViewModelLocator.Current.NoificationViewModel.NotificationHandler += ViewModel_NotificationHandler;
            ViewModel.FreqCounterViewModel.RemoveSelectionHandler += FreqCounterViewModel_RemoveSelectionHandler;
        }

        private void FreqCounterViewModel_RemoveSelectionHandler()
        {
            DrawingRectangle.ClearCanvas();
        }

        private void ViewModel_NotificationHandler(string message)
        {
            Notif.Show(message,2000);
        }

      

        // For more on the MediaPlayer and adjusting controls and behavior see https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/media-playback
        // The DisplayRequest is used to stop the screen dimming while watching for extended periods
        private DisplayRequest _displayRequest = new DisplayRequest();
        private bool _isRequestActive = false;

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender is MediaPlaybackSession playbackSession && playbackSession.NaturalVideoHeight != 0)
            {
                if (playbackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    if (!_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestActive();
                            _isRequestActive = true;
                        });
                    }
                }
                else
                {
                    if (_isRequestActive)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            _displayRequest.RequestRelease();
                            _isRequestActive = false;
                        });
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Mpe.MediaPlayer.Pause();
            Mpe.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
            ViewModel.MediaPlayerViewModel.DisposeSource();
        }

       
    }
}
