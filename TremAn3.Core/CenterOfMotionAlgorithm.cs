using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public class CenterOfMotionAlgorithm
    {
        public CenterOfMotionAlgorithm(int width, int height, double frameRate)
        {
            this.frameRate = frameRate;
            previousValueX = width / 2d;//previous is set to half in case of first and second frame are same
            previousValueY = height / 2d;
            vecOfx = Enumerable.Repeat(Enumerable.Range(0, width), height).SelectMany(x => x).ToList();
            vecOfy = Enumerable.Range(0, height).Select(x => Enumerable.Repeat(x, width)).SelectMany(x => x).ToList();
        }

        public byte[] Frame1 { get; set; } 
        public byte[] Frame2 { get; set; }

        readonly double frameRate;

        readonly List<int> vecOfx;
        readonly List<int> vecOfy;
        List<double> listComX = new List<double>();
        List<double> listComY = new List<double>();

        double previousValueX;
        double previousValueY;
        public void GetComFromCurrentFrames()
        {
            double comX = previousValueX;//
            double comY = previousValueY;
            var diff = Frame2.Zip(Frame1, (f2, f1) => (double)f2 - f1).ToArray();
            var max = diff.Max();
            var min = diff.Min();
            if (max != 0 || min != 0)//frame 1 is different than frame 2
            {
                //normalize frames
                var diffNorm = diff.Select(x => (x - min) / (max - min));
                //now "pixels" are in range from  0 to 1
                var mean = diffNorm.Average();

                var diffNormMultX = diffNorm.Zip(vecOfx, (di, vx) => di * vx);
                comX = diffNormMultX.Average() / mean;
                previousValueX = comX;

                var diffNormMultY = diffNorm.Zip(vecOfy, (di, vy) => di * vy);
                comY = diffNormMultX.Average() / mean;
                previousValueY = comY;
            }

            listComX.Add(comX);
            listComY.Add(comY);

        }

        public double GetMainFreqFromComLists()
        {
            //returned value is max from average of two spectrums
            FftResult fftX = Fft.GetAmpSpectrumAndMax(frameRate, listComX);
            FftResult fftY = Fft.GetAmpSpectrumAndMax(frameRate, listComY);
            List<double> avgSpecList = new List<double>();
            for (int i = 0; i < listComX.Count;i++)
            {
                double avg = (fftX.Values[i] + fftY.Values[i])/2;
                avgSpecList.Add(avg);
            }
            int maxIndex = avgSpecList.IndexOf( avgSpecList.Max());
            return fftX.Frequencies[maxIndex]; 
        }
    }
}
