using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TremAn3.Helpers
{
  public  class SupportedFormatsHelper
    {
        public static string[] GetSupportetVideoFormats()
        {
            //this should be same as in appxmanifest
            string[] supportedFileTypes = { ".mov", ".mp4", ".wmv", ".avi", ".flv",".mpg"};//has to be lowercase
            return supportedFileTypes;
        }
    }
    public static class FileExtensions
    {
        public static bool IsFileSupported(this StorageFile sf )
        {
            if (sf == null)
                return false;
           return SupportedFormatsHelper.GetSupportetVideoFormats().Contains(sf.FileType.ToLower());
        }

    }
}
