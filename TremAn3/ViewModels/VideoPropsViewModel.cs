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
        public async Task UpdateVideoPropsByStorageFile(StorageFile file)
        {
            CurrentFile = file;
            CurrentVideoFileProps = await CurrentFile.Properties.GetVideoPropertiesAsync();
            CurrentVideoFileBasicProps = await CurrentFile.GetBasicPropertiesAsync();
            Height = CurrentVideoFileProps.Height;
            Width = CurrentVideoFileProps.Width;
            Size = Math.Round(Convert.ToDouble( CurrentVideoFileBasicProps.Size‬) / 1048576, 2);
            Duration = CurrentVideoFileProps.Duration;
            DisplayName = CurrentFile.DisplayName;
            Name = CurrentFile.Name;
            FilePath = CurrentFile.Path;
            IDictionary<string, object> encodingProperties = await CurrentVideoFileProps.RetrievePropertiesAsync(new List<string> { "System.Video.FrameRate" });
            uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
            FrameRate = frameRateX1000 / 1000d;
        }

        public void SetToNothing()
        {
            CurrentFile = null;
            CurrentVideoFileProps = null;
            CurrentVideoFileBasicProps = null;
            Height = 0;
            Width = 0;
            Size = 0;
            Duration = TimeSpan.Zero;
            DisplayName = "";
            FilePath = "";
            FrameRate = 0;
        }

        public override string ToString() => $"{Name} | {Width}x{Height} | {FrameRate} FPS | {Size} MB";

        private StorageFile CurrentFile;
        private VideoProperties CurrentVideoFileProps { get; set; }
        private BasicProperties CurrentVideoFileBasicProps { get; set; }

        private uint _Height;
        public uint Height
        {
            get => _Height;
            set => Set(ref _Height, value);
        }

        private uint _Width;
        public uint Width
        {
            get => _Width;
            set => Set(ref _Width, value);
        }

        private double _Size;
        /// <summary>
        /// Size in MB, rounded to 2 decimals
        /// </summary>
        public double Size
        {
            get => _Size;
            set => Set(ref _Size, value);
        }

        private string _DisplayName; 
        public string DisplayName
        {
            get => _DisplayName;
            set => Set(ref _DisplayName, value);
        }
        private string _Name;
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        private string _FilePath;
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        private double _FrameRate;
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
