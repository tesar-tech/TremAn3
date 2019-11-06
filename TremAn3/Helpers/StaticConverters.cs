using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace TremAn3.Helpers
{
   public static class StaticConverters
    {

        public static string DoubleToTimeConverter(double timeInSeconds)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(timeInSeconds);
            string withOrOutMinutes = timespan.TotalMinutes >= 1 ? "mm':'ss'.'ff" : "ss'.'ff";
            return timespan.ToString(withOrOutMinutes);
        }

        public static Visibility BoolToVisiblity(bool boolVal) => boolVal ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility BoolToVisiblityInverse(bool boolVal) => BoolToVisiblity(!boolVal);
        
        public static bool InverseBool(bool val) => !val;



        public static Visibility FreqDoubleToVisibility(double val) => val < 0? Visibility.Collapsed : Visibility.Visible;


        //public static Visibility MoreThanZeroToCollapsed(int count) => count > 0 ? Visibility.Collapsed : Visibility.Visible;
    }
}
