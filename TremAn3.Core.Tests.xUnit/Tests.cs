using System;
using System.Collections.Generic;
using TremAn3.Core;
using Xunit;

namespace TremAn3.Core.Tests.XUnit
{
    // TODO WTS: Add appropriate unit tests.
    public class Tests
    {
        //List<double> Vector1 = new List<double>();
        //List<double> Vector2 = new List<double>();

        /*public Tests()
        {
            List<double> vector = new List<double>();
            double fs = 100;
            double xStep = 1 / fs;

            double y;
            double f = 13;
            for (double xx = -10; xx <= 10; xx += xStep)
            {
                y = Math.Sin(2 * Math.PI * f * xx);
                vector.Add(y);
            }

            Vector1 = vector;

        }*/
        public List<double> vytvorVektor(double fs)
        {
            List<double> vector = new List<double>();
            double xStep = 1 / fs;

            double y;
            double f = 13;
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
            List<double> Vector = vytvorVektor(100);
            result = Fft.GetAmpSpectrumAndMax(100, Vector);
            FftResult fft = null;
            Assert.Equal(result, fft);
        }

        [Fact]
        public void Test_GetAmpSpectrumAndMax_ShortVec()
        {
            FftResult result = new FftResult
            {
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            List<double> Vector = vytvorVektor(1);
            result = Fft.GetAmpSpectrumAndMax(1, Vector);


            FftResult fft = new FftResult
            {
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            fft.Values.Add(0);fft.Values.Add(3.25386894911750e-13);fft.Values.Add(1.49260991996402e-13);
            fft.Values.Add(1.59397837601688e-14);fft.Values.Add(1.36200267478192e-13);fft.Values.Add(1.26804825196142e-13);
            fft.Values.Add(3.33681824285738e-15);fft.Values.Add(1.45999479203236e-13);fft.Values.Add(1.92014261124964e-13);
            fft.Values.Add(1.31116947973350e-13);fft.Values.Add(5.36703654880356e-14);
            fft.Frequencies.Add(0);fft.Frequencies.Add(0.05);fft.Frequencies.Add(0.1);fft.Frequencies.Add(0.15);fft.Frequencies.Add(0.2);
            fft.Frequencies.Add(0.25);fft.Frequencies.Add(0.3);fft.Frequencies.Add(0.35);fft.Frequencies.Add(0.4);fft.Frequencies.Add(0.45);
            fft.Frequencies.Add(0.5);
            fft.MaxIndex = 1;

            Assert.Equal(result, fft);
        }













    }
}
