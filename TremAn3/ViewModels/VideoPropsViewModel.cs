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
        public VideoProperties CurrentVideoFileProps { get; set; }
        public BasicProperties CurrentVideoFileBasicProps { get; set; }

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

        public VideoPropsViewModel()
        {

        }
    }
}
