﻿
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.ViewModels;
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
        public static Thickness DoubleToThicknessWDivisor(double val,double divisor = 1.0) => new Thickness(val/divisor);
        public static Thickness DoubleToThicknessWDivisor(int val,double divisor = 1.0) => DoubleToThicknessWDivisor((double)val,divisor);



        public static Visibility FreqDoubleToVisibility(double val) => val < 0 ? Visibility.Collapsed : Visibility.Visible;


        public static bool NullToBool(object obj) => obj != null;
        public static bool NullBoolToBool(bool? b) => b!=null && (bool)b;

        public static Brush ColorToBrush(Color color) => new SolidColorBrush(Windows.UI.Color.FromArgb(255, color.R, color.G, color.B));

        public static Symbol IsPlayingToIcon(bool isPlaying) => isPlaying ? Symbol.Pause : Symbol.Play;
        //public static Visibility MoreThanZeroToCollapsed(int count) => count > 0 ? Visibility.Collapsed : Visibility.Visible;


        public static GridLength IsListOfMeasurementsShownHeight(bool isShown) => isShown ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Pixel);
        public static GridLength HeightOfMeasurementsInMainPage(bool isShown) => isShown ? new GridLength(1, GridUnitType.Star) :  GridLength.Auto;

        public static string ToWellDateTimeString(DateTime d) => d.ToString(Defaults.DateTimeFormatForMeasurements);
        public static double SpectralAnalysisBiggerToggleToSizeOfPlot(bool? IsChecked)
        {
            if (IsChecked == null || !IsChecked.Value)
                return 100;
            return 222;
        }

        public static string DoubleDoubleToStringPercent(double val) => $"{val:00.00} % ";

        public static string DoubleFormatter(double val) => $"{val:0.00}";

        public static string DoubleToStringHz(double val) => $"{val:00.000} Hz ";
        public static string DoubleToString3Decimals(double val) => $"{val:00.000}";





    }
    public static class MathConverters
    {
        public static double Divide(double a, double b) => a / b;
    }
}
