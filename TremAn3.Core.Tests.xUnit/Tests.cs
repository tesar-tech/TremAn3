using System;
using System.Collections.Generic;
using TremAn3.Core;
using Xunit;

namespace TremAn3.Core.Tests.XUnit
{
    // TODO WTS: Add appropriate unit tests.
    public class Tests
    {
        List<double> Vector = new List<double>();
        public Tests()
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

            Vector = vector;




        }

        [Fact]
        public void Test_GetAmpSpectrumAndMax()
        {
            FftResult result = new FftResult { 
                Frequencies = new List<double>(),
                Values = new List<double>()
            };
            result = Fft.GetAmpSpectrumAndMax(100, Vector);
            FftResult fft = null;
            Assert.Equal(result, fft);




        }
    }
}
