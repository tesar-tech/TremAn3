using System;
using System.Collections.Generic;
using System.Text;

namespace TremAn3.Core
{
   public static class Fft
    {
        public static FftResult GetAmpSpectrumAndMax()
        {
            //implementation goes here
            FftResult res = null;

            return res;
        }
    }
    public class FftResult
    {
        public int MaxIndex { get; set; }
        public List<double> Frequencies { get; set; }
        public List<double> Values { get; set; }

    }
}
