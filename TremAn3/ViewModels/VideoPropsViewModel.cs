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
        public ulong Size { get; set; }
        public string DisplayName { get; set; }
        public string FilePath { get; set; }
        public double FrameRate { get; set; }


        public VideoPropsViewModel()
        {

        }
    }
}
