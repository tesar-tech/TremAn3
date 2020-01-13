using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.Helpers
{
    public static class CsvExport
    {
        public enum CsvExportStatus
        {
            Canceled, Completed,
            NotCompleted,
        }
        public static async Task<CsvExportStatus> ExportStringAsCsvAsync(string csv,string suggestedName)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" , ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = suggestedName;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, csv);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    return CsvExportStatus.Completed;
                    //this.textBlock.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    //this.textBlock.Text = "File " + file.Name + " couldn't be saved.";
                    return CsvExportStatus.NotCompleted;
                }
            }
            else
            {
                return CsvExportStatus.Canceled;
            }
        }
    }
}
