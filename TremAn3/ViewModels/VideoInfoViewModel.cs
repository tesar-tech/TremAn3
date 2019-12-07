using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Services;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace TremAn3.ViewModels
{
   public class VideoInfoViewModel : ViewModelBase
    {

        public VideoProperties CurrentVideoFileProps { get; set; }
        public string DisplayName;
        public string FilePath;
        public ulong FileSize;
        public double FrameRate;
    }
}
