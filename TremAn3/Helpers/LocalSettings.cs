using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TremAn3.Helpers
{
    public class LocalSettings
    {

        /// <summary>
        /// simple load and save to settings..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSave"></param>
        /// <param name="key"></param>
        public static void Write<T>(T valueToSave, [CallerMemberName] string key = null)
        {
            ApplicationData.Current.LocalSettings.Values[key] = valueToSave;
        }

        public static T Read<T>(T defaultValue, string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null)
                ApplicationData.Current.LocalSettings.Values[key] = defaultValue;
            return (T)ApplicationData.Current.LocalSettings.Values[key];
        }

    }
}
