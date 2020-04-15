using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace TremAn3.ViewModels
{
    public class MediaControllingViewModel : ViewModelBase
    {

        public MediaControllingViewModel()
        {
            MediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;

        }

        private async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StartTimer();
                    IsPlaying = true;
                });
            else
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StopTimer();
                    IsPlaying = false;
                });
        }


        private MediaPlayer MediaPlayer { get; set; } = new MediaPlayer();

        private bool _IsPlaying;

        public bool IsPlaying
        {
            get => _IsPlaying;
            private set => Set(ref _IsPlaying, value);
        }


        private double _PositionSeconds;

        public double PositionSeconds
        {
            get => _PositionSeconds;
            set
            {
                if (Set(ref _PositionSeconds, value))
                {
                    PositionChanged.Invoke(value);
                    if (!IsPositionChangeFromMethod)
                        PositionChangeRequest(value);
                }
            }
        }
        public event Action<double> PositionChanged;
        internal void Play()
        {
            MediaPlayer.Play();
            IsPlaying = true;
        }

        internal void Pause()
        {
            MediaPlayer.Pause();
            IsPlaying = false;
        }

        private MainViewModel MainViewModel { get => ViewModelLocator.Current.MainViewModel; }
        private MediaPlayerViewModel MediaPlayerViewModel { get => ViewModelLocator.Current.MediaPlayerViewModel; }

        //public event Action<double> ChangePositionAction;

        public bool IsPositionChangeFromMethod { get; set; }
        public void PositionChangeRequest(double requestedPosition, bool changeMpe = true)
        {

            //IsPositionChangeFromMethod = true;
            if (changeMpe)
                MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(requestedPosition);
            IsPositionChangeFromMethod = true;
            PositionSeconds = requestedPosition;
            MainViewModel.FreqCounterViewModel.SliderPlotValue = requestedPosition;
            IsPositionChangeFromMethod = false;
            //IsPositionChangeFromMethod = false;
        }

        private DispatcherTimer _timer;

        private void StartTimer()
        {
            if (_timer is null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(0.1);
                _timer.Tick += _timer_Tick;

            }
            _timer?.Start();
        }
        /// <summary>
        /// there should be only one call of this method
        /// </summary>
        internal void StopTimer()
        {
            _timer?.Stop();
        }

        private void _timer_Tick(object sender, object e)
        {
            PositionChangeRequest(MediaPlayer.PlaybackSession.Position.TotalSeconds, false);
        }

        public MediaPlayer GetMediaPlayer() => MediaPlayer;//i need it public just in one place
        internal void ChangeMediaPlayerSource(IMediaPlaybackSource source)
        {
            MediaPlayer.Source = source;
        }
    }
}
