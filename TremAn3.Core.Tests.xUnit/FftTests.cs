using System;
using System.Collections.Generic;
using TremAn3.Core;
using Xunit;
using System.Linq;

namespace TremAn3.Core.Tests.XUnit
{
    // TODO WTS: Add appropriate unit tests.
    public class Tests
    {
        private List<double> CreateVector(double fs, double f)
        {
            List<double> vector = new List<double>();
            double xStep = 1 / fs;
            double y;
            for (double xx = -10; xx <= 10; xx += xStep)
            {
                y = Math.Sin(2 * Math.PI * f * xx);
                vector.Add(y);
            }
            return vector;
        }

        [Fact]
        public void Test_GetAmpSpectrumAndMax_Null()
        {
            FftResult result = new FftResult { 
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            List<double> Vector = null;
            result = Fft.GetAmpSpectrumAndMax(100, Vector);
            FftResult fft = null;
            Assert.Equal(result, fft);
        }

        [Fact]
        public void Test_GetAmpSpectrumAndMax_ShortVec()
        {
            List<double> Vector = CreateVector(1,13); 
            FftResult fft = new FftResult
            {
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            FftResult result = new FftResult
            {
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            result = Fft.GetAmpSpectrumAndMax(1, Vector);
            for (int i = 0; i < result.Values.Count;i++)
            {
                result.Values[i] *= 1e13;
                result.Values[i] = Math.Round(result.Values[i],2,MidpointRounding.AwayFromZero);
            }
            fft.Values.Add(0);fft.Values.Add(3.25);fft.Values.Add(1.49);
            fft.Values.Add(0.16);fft.Values.Add(1.36);fft.Values.Add(1.27);
            fft.Values.Add(0.03);fft.Values.Add(1.46);fft.Values.Add(1.92);
            fft.Values.Add(1.31);fft.Values.Add(0.54);
            fft.Frequencies.Add(0);fft.Frequencies.Add(0.05);fft.Frequencies.Add(0.1);fft.Frequencies.Add(0.15);fft.Frequencies.Add(0.2);
            fft.Frequencies.Add(0.25);fft.Frequencies.Add(0.3);fft.Frequencies.Add(0.35);fft.Frequencies.Add(0.4);fft.Frequencies.Add(0.45);
            fft.Frequencies.Add(0.5);
            fft.MaxIndex = 1;
            Assert.Equal(result.Frequencies, fft.Frequencies);
            Assert.Equal(result.Values,fft.Values);
            Assert.Equal(result.MaxIndex, fft.MaxIndex);
            /*Hodnoty nejsou přesně shodné s matlabem(Matlab zaokrouhluje jinak než Math.Net)
              Math.Net počítá s větší přesností (e-21, matlab to zaokrouhlí na nulu)*/
        }


        [Fact]
        public void Test_GetAmpSpectrumAndMax_MaxIndexes()
        {
            List<double> freq_matlab = new List<double>();
            List<double> freq_math_dotnet = new List<double>();
            double f;
            for (int i = 1; i <= 45; i++)
            {
                f = i * 20;
                freq_matlab.Add(f);
                List<double> Vector = CreateVector(100, i);
                freq_math_dotnet.Add(Fft.GetAmpSpectrumAndMax(i, Vector).MaxIndex);
            }
            Assert.Equal(freq_matlab, freq_math_dotnet);
        }
    }
}
