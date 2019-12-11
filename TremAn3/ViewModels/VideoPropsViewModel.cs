using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace TremAn3.ViewModels
{
    public class VideoPropsViewModel : ViewModelBase
    {
        public async void UpdateVideoPropsByStorageFile(StorageFile file)
        {
            CurrentFile = file;
            CurrentVideoFileProps = await CurrentFile.Properties.GetVideoPropertiesAsync();
            CurrentVideoFileBasicProps = await CurrentFile.GetBasicPropertiesAsync();
            Height = CurrentVideoFileProps.Height;
            Width = CurrentVideoFileProps.Width;
            Size = CurrentVideoFileBasicProps.Size / 1024;
            DisplayName = CurrentFile.DisplayName;
            FilePath = CurrentFile.Path;
            IDictionary<string, object> encodingProperties = await CurrentVideoFileProps.RetrievePropertiesAsync(new List<string> { "System.Video.FrameRate" });
            uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
            FrameRate = frameRateX1000 / 1000d;
        }

        private StorageFile CurrentFile;
        private VideoProperties CurrentVideoFileProps { get; set; }
        private BasicProperties CurrentVideoFileBasicProps { get; set; }

        public uint _Height;
        public uint Height
        {
            get => _Height;
            set => Set(ref _Height, value);
        }

        public uint _Width;
        public uint Width
        {
            get => _Width;
            set => Set(ref _Width, value);
        }

        public ulong _Size;
        public ulong Size
        {
            get => _Size;
            set => Set(ref _Size, value);
        }

        public string _DisplayName; 
        public string DisplayName
        {
            get => _DisplayName;
            set => Set(ref _DisplayName, value);
        }

        public string _FilePath;
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        public double _FrameRate;
        public double FrameRate
        {
            get => _FrameRate;
            set => Set(ref _FrameRate, value);
        }

        private TimeSpan _Duration;

        public TimeSpan Duration
        {
            get => _Duration;
            set => Set(ref _Duration, value);
        }

    }
}
