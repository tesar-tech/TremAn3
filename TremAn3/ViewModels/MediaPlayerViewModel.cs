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

        //private const string DefaultSource = "ms-appx:///Assets/beru.wmv";




        private IMediaPlaybackSource _source;

        public IMediaPlaybackSource Source
        {
            get { return _source; }
           private set {
                Set(ref _source, value);
            }
        }


        public VideoPropsViewModel VideoPropsViewModel { get; set; } = new VideoPropsViewModel();

        public StorageFile CurrentStorageFile { get; private set; }

        public async Task ChangeSourceAsync(StorageFile file)
        {
           if (file != null)
            {
                Source = MediaSource.CreateFromStorageFile(file);
                CurrentStorageFile = file;
                await VideoPropsViewModel.UpdateVideoPropsByStorageFile(file);
            }
        }

        internal async Task SetDefaultSourceAsync()
        {
            var defaultStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/beru.wmv"));
            await ChangeSourceAsync(defaultStorageFile);
        }


        private MainViewModel ParentVm { get => ViewModelLocator.Current.MainViewModel;  }
        public MediaPlayerViewModel()
        {
        }

        public void DisposeSource()
        {
            var mediaSource = Source as MediaSource;
            mediaSource?.Dispose();
            Source = null;
        }

        private double _PositionSeconds;

        public double PositionSeconds
        {
            get => _PositionSeconds;
            private set => Set(ref _PositionSeconds, value);
        }

        public event Action<double> ChangePositionAction;

        public bool IsPositionChangeFromMethod { get; set; }
        public void PositionChangeRequest(double requestedPosition, bool isFromMpe = false)
        {
            //IsPositionChangeFromMethod = true;
            if(!isFromMpe)
            ChangePositionAction.Invoke(requestedPosition);
            PositionSeconds = requestedPosition;
            ParentVm.FreqCounterViewModel.SliderPlotValue = requestedPosition;
            //IsPositionChangeFromMethod = false;
            
        }



    }
}
