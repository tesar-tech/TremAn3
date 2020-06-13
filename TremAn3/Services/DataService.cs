using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Helpers;
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
        public async Task<StorageFile> OpenFileDialogueAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            SupportedFormatsHelper.GetSupportetVideoFormats().ToList().ForEach(x => openPicker.FileTypeFilter.Add(x));

            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

    }
}
