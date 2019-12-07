using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Services;
using Windows.Storage.FileProperties;

namespace TremAn3.ViewModels
{
   public class VideoInfoViewModel : ViewModelBase
    {

        public VideoProperties CurrentVideoFileProps { get; set; }
        public object GetProperties()
        {

            //MPVM = MainViewModel.MediaPlayerViewModel;
            //var MPVM = MainViewModel.MediaPlayerViewModel;
            //Windows.Storage.FileProperties.VideoProperties vidprops = await CurrentStorageFile.GetVideoPropertiesAsync();
            //VideoProperties vidprops = MPVM.CurrentVideoFileProps;
            //string Duration = vidprops.Duration.ToString();
            //return Duration;  
            return "Testdata";
        }
    }
}
