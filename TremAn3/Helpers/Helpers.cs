using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace TremAn3.Helpers
{
    public  class Helpers
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
        public static string MeasurementsFolderName { get; } = "AllMeasurements";
    }


    }
