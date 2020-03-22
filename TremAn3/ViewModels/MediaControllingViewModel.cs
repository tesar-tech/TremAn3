using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace TremAn3.ViewModels
{
   public class MediaControllingViewModel:ViewModelBase
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
                });
            else
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StopTimer();
                });
        }

        private MediaPlayer MediaPlayer { get; set; } = new MediaPlayer();


        private double _PositionSeconds;

        public double PositionSeconds
        {
            get => _PositionSeconds;
            private set => Set(ref _PositionSeconds, value);
        }
        private MainViewModel MainViewModel { get => ViewModelLocator.Current.MainViewModel; }
        private MediaPlayerViewModel MediaPlayerViewModel { get => ViewModelLocator.Current.MediaPlayerViewModel; }

        public event Action<double> ChangePositionAction;

        public bool IsPositionChangeFromMethod { get; set; }
        public void PositionChangeRequest(double requestedPosition, bool changeMpe = true)
        {

            //IsPositionChangeFromMethod = true;
            if (changeMpe)
                MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(requestedPosition);
           PositionSeconds = requestedPosition;
            IsPositionChangeFromMethod = true;
           MainViewModel.FreqCounterViewModel.SliderPlotValue = requestedPosition;
            IsPositionChangeFromMethod = false;
            //IsPositionChangeFromMethod = false;

        }

        private DispatcherTimer _timer;

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(0.1);
                _timer.Tick += _timer_Tick;

            }
            if (!_timer.IsEnabled)//zjistí se, jestli se timer nesnaží být zapnut pokud už je zapnut - patologický stav
            {
                _timer.Start();
            }
            else
            {
            }
            //    throw new InvalidOperationException("Timer by neměl být zapnut, pokud je zapnut (2x by se přidal odběr událostí )!!");
        }
        /// <summary>
        /// there should be only one call of this method
        /// </summary>
        internal void StopTimer()
        {
            //if (_timer != null)//což se může stát v případě "žádného videa"
            //{
                _timer?.Stop();
            //}
        }

        private void _timer_Tick(object sender, object e)
        {
            //if (!_sliderpressed)
            PositionChangeRequest(MediaPlayer.PlaybackSession.Position.TotalSeconds, false);
        }

        public MediaPlayer GetMediaPlayer() => MediaPlayer;//i need it public just in one place
        internal void ChangeMediaPlayerSource(IMediaPlaybackSource source)
        {
            MediaPlayer.Source = source;
        }
    }
}
