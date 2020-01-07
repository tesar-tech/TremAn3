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
        //public static List<double> ComputeFftDuringSignal(double fs, List<double> vector, int windowSizeSamples, int step, bool removeAvgInSegment = true)
        //{
        //    if (vector == null || !vector.Any())
        //        return null;
        //    if (vector.Count == 1)
        //        throw new ArgumentException("Vector must have more than 1 value", nameof(vector));
        //    if (windowSizeSamples > vector.Count)
        //        throw new ArgumentException("Size of window must be smaller than count of values in vector", nameof(windowSizeSamples));
        //    if (fs <= 0)
        //        throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
        //    if (windowSizeSamples <= 0)
        //        throw new ArgumentException("Size of window cannot be less than or equal to zero", nameof(windowSizeSamples));
        //    if(step <= 0)
        //        throw new ArgumentException("Step cannot be less than or equal to zero", nameof(step));
        //    //if(windowSize + step > vector.Count)
        //    //    throw new ArgumentException("WindowSize + step must be smaller than count of values in vector", nameof(step));

        //    var vectorToBeCut = new List<double>(vector);//copy list 
        //    var fftList = new List<double>();
        //    double[] segment = new double[windowSizeSamples];
        //    while (vectorToBeCut.Count > (windowSizeSamples + step)-1)
        //    {
        //        vectorToBeCut.CopyTo(0, segment, 0, windowSizeSamples);
        //        vectorToBeCut.RemoveRange(0, step);
        //        FftResult res = GetAmpSpectrumAndMax(fs, segment.ToList(),removeAvgInSegment);
        //        var maxFromSegment = res.Frequencies[res.MaxIndex];//get max freq from current segment
        //        fftList.Add(maxFromSegment);
        //    }
        //    return fftList;
        //}

        public static List<double> ComputeFftDuringSignalForTwoSignals(double fs, List<double> vector1, List<double> vector2, int windowSizeSamples, int step, bool removeAvgInSegment = true)
        {
            //todo vectors have to be the same length
            //todo: do this for both vectors
            //if (vector == null || !vector.Any())
            //    return null;
            //if (vector.Count == 1)
            //    throw new ArgumentException("Vector must have more than 1 value", nameof(vector));
            //if (windowSizeSamples > vector.Count)
            //    throw new ArgumentException("Size of window must be smaller than count of values in vector", nameof(windowSizeSamples));
            if (fs <= 0)
                throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
            if (windowSizeSamples <= 0)
                throw new ArgumentException("Size of window cannot be less than or equal to zero", nameof(windowSizeSamples));
            if (step <= 0)
                throw new ArgumentException("Step cannot be less than or equal to zero", nameof(step));
            //if(windowSize + step > vector.Count)
            //    throw new ArgumentException("WindowSize + step must be smaller than count of values in vector", nameof(step));

            var vectorToBeCut1 = new List<double>(vector1);//copy list 
            var vectorToBeCut2 = new List<double>(vector2);//copy list 
            var fftList = new List<double>();
            double[] segment1 = new double[windowSizeSamples];
            double[] segment2 = new double[windowSizeSamples];
            while (vectorToBeCut1.Count > (windowSizeSamples + step) - 1)
            {
                vectorToBeCut1.CopyTo(0, segment1, 0, windowSizeSamples);
                vectorToBeCut1.RemoveRange(0, step);

                vectorToBeCut2.CopyTo(0, segment2, 0, windowSizeSamples);
                vectorToBeCut2.RemoveRange(0, step);

                FftResult res1 = GetAmpSpectrumAndMax(fs, segment1.ToList(), removeAvgInSegment);
                FftResult res2 = GetAmpSpectrumAndMax(fs, segment2.ToList(), removeAvgInSegment);


                List<double> avgSpecList = new List<double>();
                for (int i = 0; i < res1.Values.Count; i++)
                {
                    double avg = (res1.Values[i] + res2.Values[i]) / 2;
                    avgSpecList.Add(avg);
                }
                int maxIndex = avgSpecList.IndexOf(avgSpecList.Max());
                double maxFromSegment = res1.Frequencies[maxIndex];
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
