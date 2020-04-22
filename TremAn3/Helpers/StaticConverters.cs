
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

        public static Visibility NonZeroRectToVisibility((uint, uint, uint, uint) rect) => (rect.Item1 + rect.Item2 + rect.Item3 + rect.Item4) != 0 ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility BoolToVisibility(bool boolVal) => boolVal ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility BoolToVisiblityInverse(bool boolVal) => BoolToVisibility(!boolVal);
        //public static Visibility NullToVisibility(object obj) => obj != null ? Visibility.Visible : Visibility.Collapsed;

        public static bool InverseBool(bool val) => !val;
        public static Thickness IntToThickness(double val) => new Thickness(val);
        public static Thickness DoubleToThicknessWDivisor(double val,double divisor = 1) => new Thickness(val/divisor);



        public static Visibility FreqDoubleToVisibility(double val) => val < 0 ? Visibility.Collapsed : Visibility.Visible;


        public static bool NullToBool(object obj) => obj != null;
        public static bool NullBoolToBool(bool? b) => b!=null && (bool)b;

        public static Brush ColorToBrush(Color color) => new SolidColorBrush(Windows.UI.Color.FromArgb(255, color.R, color.G, color.B));

        public static Symbol IsPlayingToIcon(bool isPlaying) => isPlaying ? Symbol.Pause : Symbol.Play;
        //public static Visibility MoreThanZeroToCollapsed(int count) => count > 0 ? Visibility.Collapsed : Visibility.Visible;

    }
    public static class MathConverters
    {
        public static double Divide(double a, double b) => a / b;
    }
}
