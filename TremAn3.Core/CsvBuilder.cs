using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public static class CsvBuilder
    {
        public static string GetCvsFromOneX_MultipleY(List<double> xs, List<List<double>> multiple_ys, (string DecimalSeparator, string CsvElementSeparator) separators, IEnumerable<string> headers)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();

            nfi.NumberDecimalSeparator = separators.DecimalSeparator;
            var s = separators.CsvElementSeparator.Replace(@"\t", "\t");

            StringBuilder builder = new StringBuilder(string.Join(s, headers));
            builder.Append("\n");

            int dataLength = multiple_ys[0].Count;

            for (int i = 0; i < dataLength; i++)
            {
                builder.Append($"{xs[i]}");//fill x value
                foreach (var y in multiple_ys)
                {
                    builder.Append($"{s}{y[i].ToString("F6", nfi)}");//fill y values
                }// need to round it bcs of small number in E-08 format (nan in excel)
                builder.Append("\n");//add new line
            }
            return builder.ToString();
        }
        
       public static string GetCsvFromListOfLists(List<List<double>> values, (string DecimalSeparator, string CsvElementSeparator) separators, IEnumerable<string> headers)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = separators.DecimalSeparator
            };

            string separator = separators.CsvElementSeparator.Replace(@"\t", "\t");
            var headersList = headers.ToList();
            StringBuilder builder =new StringBuilder(string.Empty);
    
            // Add headers to CSV
            builder.AppendLine(string.Join(separator, headersList));

            // Find the longest row
            int maxRowLength = values.Max(row => row.Count);

            // Iterate through each row
            for (int i = 0; i < maxRowLength; i++)
            {
                // Iterate through each list to build the row
                var rowValues = values.Select(list => i >= list.Count ? string.Empty : list[i].ToString("F6", nfi)).ToList();

                // Combine the row values separated by the CSV element separator and append a new line
                builder.AppendLine(string.Join(separator, rowValues));
            }
    
            return builder.ToString();
        }
    }
}
