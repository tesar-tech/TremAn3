using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TremAn3.Core.SignalProcessing
{
    public static class SignalProcessingHelpers
    {
        //vector = vector(1:N/2+1);
        //in: [1 2 3 4 5] out: [1 2 3]
        //in: [1 2 3 4 5 6] out: [1 2 3 4]
        // float [] ar = new float []{1,2,3,4,5};
        // ar.GetHalfForPsd() 
        public static IEnumerable<double> GetHalfForPsd(this IEnumerable<double> vector)
        => vector.Take(vector.Count().HalfForPsd()).ToArray();

        public static int HalfForPsd(this int num)
        => (int)Math.Round(num / 2.0, MidpointRounding.ToEven) + 1;

        public static double[] GetFrequencies(int count, double fs)
         => Generate.LinearSpaced(count, 0, fs / 2).ToArray();

        public static IEnumerable<double> Abs(this IEnumerable<Complex32> vector)
            => vector.Select(x => x.Magnitude).ToD();
        public static List<double> RemoveAverage(this List<double> array)
        {
            if(array.Count<1) return array;
            var average = array.Average();
            var woutAvg = array.Select(x => x - average).ToList();
            return woutAvg;

        }

        public static double[] ToD(this float[] arr) => arr.Select(x => (double)x).ToArray();
        public static double[] ToD(this IEnumerable<float> enu) => enu.Select(x => (double)x).ToArray();
        public static double[] ToD(this IEnumerable<double> enu) => enu.Select(x => (double)x).ToArray();


        public static float[] ToF(this double[] arr) => arr.Select(x => (float)x).ToArray();
        public static float[] ToF(this IEnumerable<double> enu) => enu.Select(x => (float)x).ToArray();
        public static float[] ToF(this IEnumerable<float> enu) => enu.Select(x => (float)x).ToArray();
    }
}
