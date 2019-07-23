using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.ViewModels;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace TremAn3.Services
{
    public class DataService
    {
        public async Task<IStorageFile> OpenFileDialogueAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".mov");
            openPicker.FileTypeFilter.Add(".flv");

            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
            /*
             * if (file != null)
            {
                MediaPlayerViewModel.ChangeSource(file);
                //MediaPlayerViewModel.Source = MediaSource.CreateFromStorageFile(file);
            }*/
        }

        public MediaPlayerViewModel MediaPlayerViewModel { get; set; } = new MediaPlayerViewModel();
    }
}
