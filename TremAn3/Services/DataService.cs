﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.Helpers;
using TremAn3.ViewModels;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using static TremAn3.Services.JsonService;

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
        public string SaveOpenedFileToMru(StorageFile file)
        {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            string mruToken = mru.Add(file);
            LocalSettings.Write(mruToken, lastOpenedMruKey);
            return mruToken;
        }

        internal async Task SaveMeasurementResults(MeasurementModel measurementModel, StorageFile currentStorageFile, string currentMruToken)
        {
            //resultsViewModel.Id = resultsViewModel.Id == Guid.Empty ? Guid.NewGuid(): resultsViewModel.Id;
            var allMeasurementsFolder = await GetFolder_AllMeasurements();
            StorageFolder measurementsFolderForVideo = await GetFolderForVideo(allMeasurementsFolder, currentStorageFile, currentMruToken);
            string measurementFolderAndFIleName = $"measurement_{DateTime.Now:yyyy-MM-dd_HH-mm-ss.ff}_{measurementModel.Id}";
            StorageFolder folderForMeasurement = await measurementsFolderForVideo.CreateFolderAsync(measurementFolderAndFIleName, CreationCollisionOption.OpenIfExists);
            await JsonServices.WriteToJsonFile(folderForMeasurement, $"{measurementFolderAndFIleName}.json", measurementModel);


            //csvs will have similar structure of filename 

        }

        public async Task<List<MeasurementModel>> GetPastMeasurements(StorageFile currentStorageFile, string currentMruToken)
        {
            List<MeasurementModel> measurementsModels = new List<MeasurementModel>();
            var allMeasurementsFolder = await GetFolder_AllMeasurements();
            StorageFolder measurementsFolderForVideo = await GetFolderForVideo(allMeasurementsFolder, currentStorageFile, currentMruToken);
            var measurementsFolders = await measurementsFolderForVideo.GetFoldersAsync();
            foreach (var mesFolder in measurementsFolders)
            {
                var jsonFile = (await mesFolder.GetFilesAsync()).FirstOrDefault();
                if (jsonFile is null) continue;
                MeasurementModel measModel = await JsonServices.ReadFromJsonFile<MeasurementModel>(jsonFile);
                measurementsModels.Add(measModel);
            }

            return measurementsModels;
        }


        /// <summary>
        /// Folder inside AllMeasurementsFolder. This folder is for one video and multiple measurements
        /// </summary>
        /// <param name="measurementsFolder"></param>
        /// <param name="currentStorageFile"></param>
        /// <param name="currentMruToken"></param>
        /// <returns></returns>
        private async Task<StorageFolder> GetFolderForVideo(StorageFolder measurementsFolder, StorageFile currentStorageFile, string currentMruToken)
        {
            string videoFolderName = $"video_{GetPathToSourceForFileName(currentStorageFile)}_{currentMruToken}";
            StorageFolder videoFolder = await measurementsFolder.CreateFolderAsync(videoFolderName, CreationCollisionOption.OpenIfExists);
            return videoFolder;
        }
        /// <summary>
        /// Folder in C:\Users\<user>\AppData\Local\Packages\....PackageName....\LocalState
        /// In this folder there is one folder for every video opened
        /// </summary>
        /// <returns></returns>
        private static async Task<StorageFolder> GetFolder_AllMeasurements()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder recentFolder = await localFolder.CreateFolderAsync(Defaults.MeasurementsFolderName, CreationCollisionOption.OpenIfExists);
            return recentFolder;
        }

        /// <summary>
        /// cleans the filename and create folder name from it (probalbly not neccessary)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetPathToSourceForFileName(StorageFile file)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanFileName = new string(file.Name.Select(m => invalidChars.Contains(m) ? '_' : m).ToArray());
            return cleanFileName;
        }

    }
}
