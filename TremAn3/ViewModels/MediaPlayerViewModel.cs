using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;

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


        public VideoPropsViewModel VideoPropsViewModel = new VideoPropsViewModel();

        public StorageFile CurrentStorageFile { get; private set; }

        public async Task ChangeSourceAsync(StorageFile file)
        {
            if (file != null)
            {
                Source = MediaSource.CreateFromStorageFile(file);
                CurrentStorageFile = file;
                VideoPropsViewModel.CurrentVideoFileProps = await CurrentStorageFile.Properties.GetVideoPropertiesAsync();
                VideoPropsViewModel.CurrentVideoFileBasicProps = await CurrentStorageFile.GetBasicPropertiesAsync();
                VideoPropsViewModel.Size = VideoPropsViewModel.CurrentVideoFileBasicProps.Size / 1000;
                VideoPropsViewModel.DisplayName = CurrentStorageFile.DisplayName;
                VideoPropsViewModel.FilePath = CurrentStorageFile.Path;
                IDictionary<string, object> encodingProperties = await VideoPropsViewModel.CurrentVideoFileProps.RetrievePropertiesAsync(new List<string> { "System.Video.FrameRate" });
                uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
                VideoPropsViewModel.FrameRate = frameRateX1000 / 1000d;
            }
        }

        internal async Task SetDefaultSourceAsync()
        {
            var defaultStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/beru.wmv"));
            await ChangeSourceAsync(defaultStorageFile);
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
