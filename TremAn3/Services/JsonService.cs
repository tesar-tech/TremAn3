using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TremAn3.Services
{
    internal class JsonService
    {
        public static class JsonServices
        {
            public static async Task WriteToJsonFile<T>(StorageFolder folder, string fileName, T objectToWrite) where T : new()
            {
                string json = JsonConvert.SerializeObject(objectToWrite);

                try
                {
                    StorageFile sampleFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(sampleFile, json);
                }
                catch (FileLoadException ex)
                {
                    // ViewModelLocator.Current.StoreLoggingService.Log("FileLoadExceptionInJsonServiceWriteToJsonFile", ("exception", ex.Message));
                    //sometimes it just happens that txt file cannot be deleted on collision..
                }
                catch (Exception ex)
                {
                   // ViewModelLocator.Current.StoreLoggingService.Log("AnotherExceptionInJsonServiceWriteToJsonFile", ("exception", ex.Message));
                }
            }
            public static async Task WriteToJsonFile<T>(StorageFile file, T objectToWrite) where T : new()
            {
                string json = JsonConvert.SerializeObject(objectToWrite);
                await FileIO.WriteTextAsync(file, json);
            }

            public static async Task<T> ReadFromJsonFile<T>(StorageFile file) where T : new()
            {
                string fileContents = "";

                try
                {
                    if (file != null)
                    {
                        //using (var co = await file.OpenReadAsync())
                        //fileContents = await co.ReadTextAsync();
                        fileContents = await FileIO.ReadTextAsync(file);
                    }
                    else
                        return default(T);
                    T data = JsonConvert.DeserializeObject<T>(fileContents);
                    return data;

                    // Data is contained in timestamp
                }
                catch (Exception)
                {
                    return default(T);//není důvod, aby to sem padlo.
                }
            }

        }
    }
}
