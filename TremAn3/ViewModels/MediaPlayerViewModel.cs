using System;

using GalaSoft.MvvmLight;

using Windows.Media.Core;
using Windows.Media.Playback;

namespace TremAn3.ViewModels
{
    public class MediaPlayerViewModel : ViewModelBase
    {
        // TODO WTS: Set your default video and image URIs

        private const string DefaultSource = "ms-appx:///Assets/beru.wmv";

        private IMediaPlaybackSource _source;

        public IMediaPlaybackSource Source
        {
            get { return _source; }
           private set { Set(ref _source, value); }
        }

        public MediaPlayerViewModel()
        {
            Source = MediaSource.CreateFromUri(new Uri(DefaultSource));
            //PosterSource = DefaultPoster;
        }

        public void DisposeSource()
        {
            var mediaSource = Source as MediaSource;
            mediaSource?.Dispose();
            Source = null;
        }
    }
}
