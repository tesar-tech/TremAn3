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
        //Method has optional bool argument removeAverage, because in other methods it accepts already averaged vector.
        public static FftResult GetAmpSpectrumAndMax(double fs, List<double> vector,bool removeAverage = true)
        {
            if (vector == null || !vector.Any())
                return null;
            if (vector.Count == 1)
                throw new ArgumentException("Vector must have more than 1 value",nameof(vector));
            if (fs <= 0)
                throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
                
            Complex32[] vec = new Complex32[vector.Count];
            int i = 0;
            double vecAvg = removeAverage ? vector.Average():0;//don't compute avg if not needed
            foreach (var ve in vector)
            {
                vec[i] = removeAverage ? new Complex32((float)(ve - vecAvg), 0) : new Complex32((float)ve, 0);
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
        public static List<double> ComputeFftDuringSignal(double fs, List<double> vector, int windowSize, int step)
        {
            if (vector == null || !vector.Any())
                return null;
            if (vector.Count == 1)
                throw new ArgumentException("Vector must have more than 1 value", nameof(vector));
            if (windowSize > vector.Count)
                throw new ArgumentException("Size of window must be smaller than count of values in vector", nameof(windowSize));
            if (fs <= 0)
                throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
            if (windowSize <= 0)
                throw new ArgumentException("Size of window cannot be less than or equal to zero", nameof(windowSize));
            if(step <= 0)
                throw new ArgumentException("Step cannot be less than or equal to zero", nameof(step));
            //if(windowSize + step > vector.Count)
            //    throw new ArgumentException("WindowSize + step must be smaller than count of values in vector", nameof(step));

            var vectorToBeCut = new List<double>(vector);//copy list 
            var fftList = new List<double>();
            double[] segment = new double[windowSize];
            while (vectorToBeCut.Count > (windowSize + step)-1)
            {
                vectorToBeCut.CopyTo(0, segment, 0, windowSize);
                vectorToBeCut.RemoveRange(0, step);
                FftResult res = GetAmpSpectrumAndMax(fs, segment.ToList());
                var maxFromSegment = res.Frequencies[res.MaxIndex];//get max freq from current segment
                fftList.Add(maxFromSegment);
            }
            return fftList;
        }
    }
    public class FftResult
    {
        public int MaxIndex { get; set; }
        public List<double> Frequencies { get; set; } = new List<double>();
        public List<double> Values { get; set; } = new List<double>();
    }
}
