using System;
using System.Collections.Generic;
using TremAn3.Core;
using Xunit;
using System.Linq;

namespace TremAn3.Core.Tests.XUnit
{
    // TODO WTS: Add appropriate unit tests.
    public class FftTests
    {
        /// <summary>
        /// creates sin vector from time -10 to time 10s with expected sampling freq and frequency of sinus
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private List<double> CreateVector(double fs, double frequency)
        {
            List<double> vector = new List<double>();
            double xStep = 1 / fs;
            double y;
            for (double xx = -10; xx <= 10; xx += xStep)
            {
                y = Math.Sin(2 * Math.PI * frequency * xx);
                vector.Add(y);
            }
            return vector;
        }

        [Fact]
        public void GetAmpSpectrumAndMax_nullVector_returnsNull()
        {
            var result = Fft.GetAmpSpectrumAndMax(100, null);
            Assert.Null(result);
        }

        [Fact]
        public void GetAmpSpectrumAndMax_ShortVec_sameResult()
        {
            List<double> Vector = CreateVector(1, 13);
            FftResult fft = new FftResult();
            FftResult result = Fft.GetAmpSpectrumAndMax(1, Vector);
            for (int i = 0; i < result.Values.Count; i++)
            {
                result.Values[i] *= 1e13;
                result.Values[i] = Math.Round(result.Values[i], 2, MidpointRounding.AwayFromZero);
            }
            List<(double, double)> valFreqTuple = new List<(double, double)>() {
                (0d,0d),(3.25,0.05),(1.49,0.1),(0.16,0.15),(1.36,0.2),(1.27,0.25),(0.03,0.3),(1.46,0.35),(1.92,0.4),(1.31,0.45),(0.54,0.5)
            };

            valFreqTuple.ForEach(x => { fft.Values.Add(x.Item1); fft.Frequencies.Add(x.Item2); });
            fft.MaxIndex = 1;

            Assert.Equal(result.Frequencies, fft.Frequencies);
            Assert.Equal(result.Values, fft.Values);
            Assert.Equal(result.MaxIndex, fft.MaxIndex);
            /*Hodnoty nejsou přesně shodné s matlabem(Matlab zaokrouhluje jinak než Math.Net)
              Math.Net počítá s větší přesností (e-21, matlab to zaokrouhlí na nulu)*/
        }
        [Fact]
        public void GetAmpSpectrumAndMax_ShortVec1_sameResult()
        {
            List<double> vecX = new List<double>() { 319.5, 319.570564104974, 319.570564104974, 319.387187167831 };
            List<double> vecY = new List<double>() { 239.5, 239.690534471355, 239.690534471355, 239.447377199610 };
            FftResult resultX = Fft.GetAmpSpectrumAndMax(29.966, vecX);
            FftResult resultY = Fft.GetAmpSpectrumAndMax(29.966, vecY);
            resultY.Values = resultY.Values.Select(x => Math.Round(x, 7)).ToList();
            resultX.Values = resultX.Values.Select(x => Math.Round(x, 7)).ToList();
            List<double> matlabValuesX = new List<double>() { 0, 0.1964851 };
            List<double> matlabValuesY = new List<double>() { 0, 0.3089156 };
            Assert.Equal(matlabValuesX, resultX.Values);
            Assert.Equal(matlabValuesY, resultY.Values);
        }


        [Fact]
        public void GetAmpSpectrumAndMax_MultipleMaxIndexes_SameMaxIndexes()
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
        [Fact]
        public void GetAmpSpectrumAndMax_EmptyInputList_returnsNull()
        {
            List<double> vector = new List<double>();
            var result = Fft.GetAmpSpectrumAndMax(100, vector);
            Assert.Null(result);
        }
        [Fact]
        public void GetAmpSpectrumAndMax_FsNotValid0_ThrowsError()
        {
            var vector = CreateVector(100, 10);
            Assert.Throws<ArgumentException>(() => Fft.GetAmpSpectrumAndMax(0, vector));
        }
        [Fact]
        public void GetAmpSpectrumAndMax_FsNotValidLessThanZero_ThrowsError()
        {
            var vector = CreateVector(100, 10);
            Assert.Throws<ArgumentException>(() => Fft.GetAmpSpectrumAndMax(-10, vector));
        }
        /*[Fact]
        public void ComputeFftDuringSignal_ShortVec_sameResult()
        {
            List<double> Vector = CreateVector(1, 13);
            FftSpectogramResult fft = Fft.ComputeFftDuringSignal(1, Vector, 1);
        }*/
    }
}