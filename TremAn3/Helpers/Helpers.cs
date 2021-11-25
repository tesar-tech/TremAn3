using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace TremAn3.Helpers
{
    public class Helpers
    {
        public static string VersionOfApp
        {
            get
            {
                PackageVersion version = Package.Current.Id.Version;
                return $"{version.Major}.{ version.Minor}.{ version.Build}.{ version.Revision}";
            }
        }

    }


    public static class Defaults
    {
        public static string MeasurementsFolderName { get; } = "AllM";

        public static string DateTimeFormatForMeasurements => "yyyy-MM-dd HH:mm:ss";
    }

    static class Extensions
    {
        ///https://stackoverflow.com/a/31658954/1154773
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector, bool ascending = false)
        {
            List<TSource> sorted = collection.OrderBy(keySelector).ToList();
            if (!ascending) sorted.Reverse();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class PlotNameAttribute : Attribute
    {
        public string PlotName { get; private set; }

        public PlotNameAttribute(string PlotName)
        {
            this.PlotName = PlotName;
        }
    }

 

}
