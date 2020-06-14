using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// retrieve last opend storage file using most recentusedList and mru token saved in local settings
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFile> GetLastOpenedFile()
        {
            var mruToken = LocalSettings.Read("", lastOpenedMruKey);
            if (string.IsNullOrEmpty(mruToken))
                return null;
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            try
            {
                var retrievedFile = await mru.GetFileAsync(mruToken);
                return retrievedFile;
            }
            catch (FileNotFoundException)
            {
                mru.Remove(mruToken);
                LocalSettings.Write("", lastOpenedMruKey);
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// key for local settings
        /// </summary>
        readonly string lastOpenedMruKey = "lastOpenedFileToken";
        /// <summary>
        /// save file as last opened. Add to mru and add token to settings
        /// </summary>
        /// <param name="file"></param>
        public void SaveOpenedFileToMru(StorageFile file)
        {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            string mruToken = mru.Add(file);
            LocalSettings.Write(mruToken, lastOpenedMruKey);
        }

    }
}
