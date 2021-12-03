using System;
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

        internal async Task<StorageFile> GetFileByFalToken(string falToken)
        {
            var fal = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            try
            {
            var retrievedFile = await fal.GetFileAsync(falToken);
                return retrievedFile;
            }
            catch (FileNotFoundException)
            {
                fal.Remove(falToken);
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
        public (string mru, string fal) SaveOpenedFileToMruAndFal(StorageFile file)
        {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            var fal = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            string falToken = fal.Add(file);
            string mruToken = mru.Add(file);
            LocalSettings.Write(mruToken, lastOpenedMruKey);
            return (mruToken, falToken);
        }

     

        internal async Task<StorageFolder> SaveMeasurementResults(MeasurementModel measurementModel, VideoFileModel vfm)
        {
            //resultsViewModel.Id = resultsViewModel.Id == Guid.Empty ? Guid.NewGuid(): resultsViewModel.Id;
            var allMeasurementsFolder = await GetFolder_AllMeasurements();
            StorageFolder measurementsFolderForVideo = await GetFolderForVideo(allMeasurementsFolder, vfm);
            string measurementFolderAndFIleName = $"m_{DateTime.Now:yyyy-MM-dd_HH-mm-ss.ff}_{measurementModel.Id.ToString().Substring(0,8)}";
            StorageFolder folderForMeasurement = await measurementsFolderForVideo.CreateFolderAsync(measurementFolderAndFIleName, CreationCollisionOption.OpenIfExists);
            await SaveMeasurementResults(measurementModel, folderForMeasurement);//csvs will have similar structure of filename
            return folderForMeasurement;                                                                                 
        }



      


        /// <summary>
        /// save measurement to folder which has the same name as file itself.
        /// </summary>
        /// <param name="measurementModel"></param>
        /// <param name="folderForMeasurement"></param>
        /// <returns></returns>
        internal static async Task SaveMeasurementResults(MeasurementModel measurementModel, StorageFolder folderForMeasurement,bool saveOnlyTopLevelData = false)
        {
            await JsonServices.WriteToJsonFile(folderForMeasurement, GetMeasurementFileName(folderForMeasurement), measurementModel);//saves toplevel
            //saves vector data to special folder
            if(!saveOnlyTopLevelData)
            await JsonServices.WriteToJsonFile(folderForMeasurement, GetMeasurementVectorDataFileName(folderForMeasurement), measurementModel.VectorsDataModel);
        }

        private static string GetMeasurementFileName (StorageFolder continerFolder) => $"{continerFolder.Name}.json";
        private static string GetVideoModelFileName (StorageFolder videoFolder) => $"{videoFolder.Name}.json";
        private static string GetMeasurementVectorDataFileName (StorageFolder continerFolder) => $"{continerFolder.Name}_vectorData.json";

        internal async Task DeleteAllMeasurementsForCurrentVideoFile()
        {
            await _measurementsFolderForVideo.DeleteAsync();
        }


        /// <summary>
        /// called when measurement is selected to display. It loads all the (long) vectors. We don't want to load it in advance since it
        /// may contain a lots of data
        /// </summary>
        /// <param name="model"></param>
        /// <param name="folderForMeasurement"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        internal async Task LoadVectorDataToModel(MeasurementModel model, StorageFolder folderForMeasurement)
        {
            StorageFile jsonFile;
            jsonFile = await folderForMeasurement.GetFileAsync(GetMeasurementVectorDataFileName(folderForMeasurement));
            if (jsonFile is null) throw new FileNotFoundException($"No vectorData file ({jsonFile}) in {folderForMeasurement.Name} folder");
            var vectorsDataModels = await JsonServices.ReadFromJsonFile<VectorsDataModel>(jsonFile);
            model.VectorsDataModel = vectorsDataModels;
        }

        private StorageFolder _measurementsFolderForVideo;
        //public async Task<List<MeasurementViewModel>> GetPastMeasurementsForVideo(StorageFile currentStorageFile, VideoFileModel videoFileModel)
        //{
        //    var measurementsViewModels = new List<MeasurementViewModel>();
        //    var allMeasurementsFolder = await GetFolder_AllMeasurements();
        //     _measurementsFolderForVideo = await GetFolderForVideo(allMeasurementsFolder, currentStorageFile, videoFileModel);
        //    var measurementsFolders = await _measurementsFolderForVideo.GetFoldersAsync();
        //    foreach (var mesFolder in measurementsFolders)
        //    {
        //        var jsonFile = await mesFolder.GetFileAsync(GetMeasurementFileName(mesFolder));
        //        if (jsonFile is null) continue;
        //        MeasurementModel measModel = await JsonServices.ReadFromJsonFile<MeasurementModel>(jsonFile);
        //        MeasurementViewModel vm = new MeasurementViewModel(measModel);
        //        measurementsViewModels.Add(vm);
        //        vm.FolderForMeasurement = mesFolder;
        //    }

        //    return measurementsViewModels;
        //}

        public async Task<List<MeasurementViewModel>> GetAllPastMeasurements()
        {
            var measurementsViewModels = new List<MeasurementViewModel>();
            var allMeasurementsFolder = await GetFolder_AllMeasurements();
           var videoFolders =await   allMeasurementsFolder.GetFoldersAsync();

            foreach (var videoFolder in videoFolders)
            {
                var measurementsFolders = await videoFolder.GetFoldersAsync();
                var jsonFileVideoFileModel = await videoFolder.GetFileAsync(GetVideoModelFileName(videoFolder));
                VideoFileModel vfm = await JsonServices.ReadFromJsonFile<VideoFileModel>(jsonFileVideoFileModel);


                foreach (var mesFolder in measurementsFolders)
                {
                    var jsonFile = await mesFolder.GetFileAsync(GetMeasurementFileName(mesFolder));
                    if (jsonFile is null) continue;
                    MeasurementModel measModel = await JsonServices.ReadFromJsonFile<MeasurementModel>(jsonFile);
                    MeasurementViewModel vm = new MeasurementViewModel(measModel,vfm);
                    measurementsViewModels.Add(vm);
                    vm.FolderForMeasurement = mesFolder;
                }

            }
            return measurementsViewModels;
        }






        /// <summary>
        /// Folder inside AllMeasurementsFolder. This folder is for one video and multiple measurements
        /// </summary>
        private async Task<StorageFolder> GetFolderForVideo(StorageFolder measurementsFolder, VideoFileModel videoFileModel)
        {
            string videoFolderName = $"v_{GetPathToSourceForFileName(videoFileModel.Name)}_{videoFileModel.FalToken.ToString().Trim(new char[] {'{','}' })}";
            StorageFolder videoFolder = await measurementsFolder.CreateFolderAsync(videoFolderName, CreationCollisionOption.OpenIfExists);

            //when getting folder, also check for video file. This file is used for loading basic video properties
            //withour reaching the video file itself. Also stores fal token (future access list), so we can retrieve the video file
            var videoFile = await videoFolder.TryGetItemAsync(GetVideoModelFileName(videoFolder));
            if (videoFile is null)//do nothing when already created
            {
               var vf = await videoFolder.CreateFileAsync(GetVideoModelFileName(videoFolder), CreationCollisionOption.FailIfExists);
                await JsonServices.WriteToJsonFile(vf, videoFileModel);
            }

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
        private string GetPathToSourceForFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanFileName = new string(fileName.Select(m => invalidChars.Contains(m) ? '_' : m).Take(20).ToArray());
            return cleanFileName;
        }

        public async Task DeleteStoredViewModel(MeasurementViewModel vm)
        {
            await vm.FolderForMeasurement.DeleteAsync();
        }

      

    }

    public class VideoFileModel
    {

        public VideoFileModel() { }//for json
        public VideoFileModel(VideoPropsViewModel videoPropsViewModel, string currentFalToken)
        {
            Path = videoPropsViewModel.FilePath;
            Name = videoPropsViewModel.Name;
            Duration = videoPropsViewModel.Duration.TotalSeconds;
            FalToken = currentFalToken;
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public double Duration { get; set; }
        public string FalToken { get; set; }

    }
}
