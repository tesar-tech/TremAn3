using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

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
           private set {
                Set(ref _source, value);
            }
        }

        public StorageFile CurrentStorageFile { get; private set; }

        public void ChangeSource(StorageFile file)
        {
            if (file != null)
            {
                Source = MediaSource.CreateFromStorageFile(file);
                CurrentStorageFile = file;
            }
        }

        internal async Task SetDefaultSourceAsync()
        {
            Source = MediaSource.CreateFromUri(new Uri(DefaultSource));

           CurrentStorageFile =  await StorageFile.GetFileFromApplicationUriAsync (new Uri("ms-appx:///Assets/beru.wmv"));
        }

        public MediaPlayerViewModel()
        {
        }

        public void DisposeSource()
        {
            var mediaSource = Source as MediaSource;
            mediaSource?.Dispose();
            Source = null;
        }
    }
}
