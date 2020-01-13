using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
  public static  class CsvBuilder
    {
        public static string GetCsvFromTwoLists(List<double> l1, List<double> l2, (string DecimalSeparator, string CsvElementSeparator) separators, string nameCounter="", string name1="", string name2 = "")
        {

            NumberFormatInfo nfi = new NumberFormatInfo();

            nfi.NumberDecimalSeparator = separators.DecimalSeparator;
            var s = separators.CsvElementSeparator.Replace(@"\t", "\t");
            string completeString = $"{nameCounter}{s}{name1}{s}{name2}\n";
            for (int i = 0; i < l1.Count; i++)
            {
                completeString += $"{i}{s}{l1[i].ToString("F6",nfi)}{s}{l2[i].ToString("F6",nfi)}\n";
            }
            return completeString;
        }

        public static string GetCsvFromData(List<(double freq, double psd)> psdAvgData, (string DecimalSeparator, string CsvElementSeparator) separators, string v1, string v2)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = 6;//this should work, but does no...
            
            nfi.NumberDecimalSeparator = separators.DecimalSeparator;
            var s = separators.CsvElementSeparator.Replace(@"\t", "\t");
            string completeString = $"{v1}{s}{v2}\n";

            foreach (var (freq, psd) in psdAvgData)
                completeString += $"{freq.ToString("F6",nfi)}{s}{psd.ToString("F6",nfi)}\n";// need to round it bcs of small number in E-08 format (nan in excel)
            return completeString;
        }
    }
}
