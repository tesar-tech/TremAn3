using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System.Linq;
//using System.Numerics;

namespace TremAn3.Core
{
   public static class Fft
    {
        public static FftResult GetAmpSpectrumAndMax(double fs, List<double> vector)
        {
            //if (vector == null || !vector.Any())
            //    return null;
            if (fs <= 0)
                throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
                
            Complex32[] vec = new Complex32[vector.Count];
            int i = 0;
            foreach (var ve in vector)
            {
                vec[i] = new Complex32((float)ve, 0);
                i++;
            }
            Fourier.Forward(vec, FourierOptions.Matlab);

            FftResult res = new FftResult();
            res.Frequencies = new List<double>();
            res.Values = new List<double>();
            int l = (int)Math.Round((double)vec.Length / 2, MidpointRounding.AwayFromZero);
            double konec = fs / 2;
            var k = Generate.LinearSpaced(l, 0, konec);
            for (int p = 0; p < l; p++)
            {
                k[p] = Math.Round(k[p], 10);
                res.Frequencies.Add(k[p]);
                res.Values.Add(Complex32.Abs(vec[p]));
            }
            res.MaxIndex = res.Values.FindIndex(n => n == (res.Values.Max()));
            return res;
        }
    }
    public class FftResult
    {
       
        public int MaxIndex { get; set; }
        public List<double> Frequencies { get; set; } = new List<double>();
        public List<double> Values { get; set; } = new List<double>();
    }
}
