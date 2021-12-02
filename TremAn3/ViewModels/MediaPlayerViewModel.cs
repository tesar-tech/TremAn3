using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TremAn3.Services;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace TremAn3.ViewModels
{
    public class MediaPlayerViewModel : ViewModelBase
    {
        // TODO WTS: Set your default video and image URIs

        //private const string DefaultSource = "ms-appx:///Assets/beru.wmv";




        private IMediaPlaybackSource _source;

        public IMediaPlaybackSource Source
        {
            get { return _source; }
           private set {
                Set(ref _source, value);
            }
        }

        //token of the current video file used for creating folder of video
        public string CurrentMruToken { get; set; }
        public string CurrentFalToken { get; set; }

        VideoFileModel _videoFileModel;
        public VideoFileModel VideoFileModel { get => _videoFileModel ?? new VideoFileModel(VideoPropsViewModel, CurrentFalToken); }

        public VideoPropsViewModel VideoPropsViewModel { get; set; } = new VideoPropsViewModel();

        public StorageFile CurrentStorageFile { get; private set; }

        public async Task ChangeSourceAsync(StorageFile file)
        {
           if (file != null)
            {
                Source = MediaSource.CreateFromStorageFile(file);
                MediaControllingViewModel.ChangeMediaPlayerSource(Source);
                CurrentStorageFile = file;
                await VideoPropsViewModel.UpdateVideoPropsByStorageFile(file);
                _videoFileModel = null;//reset videofilemodel, it will load everything when needed
                ParentVm.RefreshTitle();
                ViewModelLocator.Current.TeachingTipsViewModel.StartIfAppropriate(3);
            }
        }

        internal async Task SetDefaultSourceAsync()
        {
            var defaultStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/beru.wmv"));
            await ChangeSourceAsync(defaultStorageFile);
        }

        public void PlayPause()
        {
            if (MediaControllingViewModel.IsPlaying)
                MediaControllingViewModel.Pause();
            else
                MediaControllingViewModel.Play();
        }

        private void Pause()
        {
            throw new NotImplementedException();
        }

        public MediaControllingViewModel MediaControllingViewModel { get => ViewModelLocator.Current.MediaControllingViewModel;  }
        public FreqCounterViewModel FreqCounterViewModel { get => ViewModelLocator.Current.FreqCounterViewModel; }
        public MainViewModel ParentVm { get => ViewModelLocator.Current.MainViewModel;  }

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
