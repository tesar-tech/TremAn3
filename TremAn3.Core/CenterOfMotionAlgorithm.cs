using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public class CenterOfMotionAlgorithm
    {
        public CenterOfMotionAlgorithm(int widthOfFrame, int heightOfFrame, double frameRate,SelectionRectangle rectangle)
        {
            if ( rectangle.IsZeroSum)
                rectangle.FullFromResolution(widthOfFrame, heightOfFrame);
            rect = rectangle;
            this.widthOfFrame = (uint)widthOfFrame;

            this.frameRate = frameRate;
            previousValueX = (rectangle.Width - 1) / 2d;//previous is set to half in case of first and second frame are same
            previousValueY = (rectangle.Height - 1) / 2d;
            vecOfx = Enumerable.Repeat(Enumerable.Range(0, (int)rectangle.Width), (int)rectangle.Height).SelectMany(x => x).ToList();
            vecOfy = Enumerable.Range(0, (int)rectangle.Height).Select(x => Enumerable.Repeat(x, (int)rectangle.Width)).SelectMany(x => x).ToList();
            startInd = this.widthOfFrame * rectangle.Y + rectangle.X;
            endInd = startInd + (rectangle.Height - 1) * this.widthOfFrame + rectangle.Width - 1;
        }

        private uint widthOfFrame;
        //private uint heightOfFrame;
        uint startInd;
        uint endInd;
        SelectionRectangle rect;
        public byte[] Frame1 { get; set; }
        public byte[] Frame2 { get; set; }

        readonly double frameRate;

        readonly List<int> vecOfx;
        readonly List<int> vecOfy;
        public List<double> listComX = new List<double>();
        public List<double> listComY = new List<double>();
        public List<(double,double)> PsdAvgData{ get; set; }
        public List<double> ListComXNoAvg { 
            get {
                var avgX = listComX.Average();
                var noavg = listComX.Select(x => x - avgX).ToList();
                return noavg;
            } }

        public List<double> ListComYNoAvg
        {
            get
            {
                var avgY= listComY.Average();
                var noavg = listComY.Select(x => x - avgY).ToList();
                return noavg;
            }
        }

        double previousValueX;
        public double previousValueY;
        public double tdiffminmaxList = 0;
        public double tdiffminmaxArr = 0;
        public double tdiffnormmenList = 0;
        public double tdiffnormmenArr = 0;
        public double tcomxcomy = 0;
        public double tcomxcomyArr = 0;
        Stopwatch sw = new Stopwatch();
        public void GetComFromCurrentARGBFrames()
        {
            double comX = previousValueX;//
            double comY = previousValueY;
            //sw.Restart();
            //var diff = Frame2.Zip(Frame1, (f2, f1) => (double)f2 - f1);

            //var max = diff.Max();
            //var min = diff.Min();
            //tdiffminmaxList += sw.ElapsedMilliseconds;
            sw.Restart();

            var diffA = new double[rect.Height*rect.Width];
            double maxA = double.MinValue;
            double minA = double.MaxValue;

            //int iter = 0;
            //for (int i = 0; i < Frame1.Length; i += 4)
            //{
            //    byte f1 = (byte)(0.2989 * Frame1[i] + 0.5870 * Frame1[i + 1] + 0.1140 * Frame1[i + 2]);//to gray
            //    byte f2 = (byte)(0.2989 * Frame2[i] + 0.5870 * Frame2[i + 1] + 0.1140 * Frame2[i + 2]);
            //    diffA[iter] = (double)f2 - f1;
            //    //min max counting
            //    maxA = maxA < diffA[iter] ? diffA[iter] : maxA;//counting max in one loop
            //    minA = minA > diffA[iter] ? diffA[iter] : minA;//counting min in one loop
            //    iter++;
            //}


           
            int iter = 0;
            for (uint s = startInd; s < endInd; s += widthOfFrame)//jumps on begining of lines of rect
            {
                for (uint c = 0; c < rect.Width; c++)//iterates on single line
                {
                    uint i = (s + c) * 4;//index to RGBA Array
                    byte f1 = (byte)(0.2989 * Frame1[i] + 0.5870 * Frame1[i + 1] + 0.1140 * Frame1[i + 2]);//to gray
                    byte f2 = (byte)(0.2989 * Frame2[i] + 0.5870 * Frame2[i + 1] + 0.1140 * Frame2[i + 2]);

                    diffA[iter] = (double)f2 - f1;
                    //max and min computation
                    maxA = maxA < diffA[iter] ? diffA[iter] : maxA;//counting max in one loop
                    minA = minA > diffA[iter] ? diffA[iter] : minA;//counting min in one loop
                    iter++;
                }
            }

         

            tdiffminmaxArr += sw.ElapsedMilliseconds;
            sw.Restart();
            if (maxA != 0 || minA != 0)//frame 1 is different than frame 2
            {
                //sw.Restart();

                ////normalize frames
                //var diffNorm = diff.Select(x => (x - min) / (max - min));

                ////now "pixels" are in range from  0 to 1
                //var mean = diffNorm.Average();
                //tdiffnormmenList += sw.ElapsedMilliseconds;
                sw.Restart();

                var diffNormA = new double[diffA.Length];
                var max_min = maxA - minA;
                double meanA = 0;


                //sw.Restart();

                //var diffNormMultX = diffNorm.Zip(vecOfx, (di, vx) => di * vx);
                //comX = diffNormMultX.Average() / mean;
                //previousValueX = comX;

                //var diffNormMultY = diffNorm.Zip(vecOfy, (di, vy) => di * vy);
                //comY = diffNormMultY.Average() / mean;
                //previousValueY = comY;

                //tcomxcomy += sw.ElapsedMilliseconds;

                double avgSumX = 0;
                double avgSumY = 0;

                for (int i = 0; i < diffNormA.Length; i++)
                {
                    diffNormA[i] = (diffA[i] - minA) / max_min;
                    meanA += diffNormA[i];
                    avgSumX += diffNormA[i] * vecOfx[i];
                    avgSumY += diffNormA[i] * vecOfy[i];

                }
                meanA /= diffNormA.Length;

                comX = avgSumX / diffNormA.Length / meanA;
                comY = avgSumY / diffNormA.Length / meanA;
                tcomxcomyArr += sw.ElapsedMilliseconds;


            }
            listComX.Add(comX);
            listComY.Add(comY);
        }

        public double GetMainFreqAndFillPsdDataFromComLists()
        {

            //var lo = "";
            //foreach (var v in listComY)
            //{
            //    lo += $",{v}";
            //}
            //remove mean from list
            var avgX = listComX.Average();
            var avgY = listComY.Average();
            var listWithoutMeanX = listComX.Select(x => x - avgX).ToList();
            var listWithoutMeanY = listComY.Select(x => x - avgY).ToList();
            //returned value is max from average of two spectrums
            FftResult fftX = Fft.GetAmpSpectrumAndMax(frameRate, listWithoutMeanX, false);// false bc avg is already removed
            FftResult fftY = Fft.GetAmpSpectrumAndMax(frameRate, listWithoutMeanY, false);

            List<double> avgSpecList = new List<double>();
            //List<(double,double)> psdAvgData = new List<(double, double)>();
            PsdAvgData = new List<(double, double)>(); 
            //List<(double,double)> psdXData = new List<(double, double)>();
            //List<(double,double)> psdYData = new List<(double, double)>();
            for (int i = 0; i < fftX.Values.Count; i++)

            {
                double avg = (fftX.Values[i] + fftY.Values[i]) / 2;
                avgSpecList.Add(avg);
                PsdAvgData.Add((fftX.Frequencies[i],avg ));

                //psdXData.Add((fftX.Frequencies[i],))
            }
            int maxIndex = avgSpecList.IndexOf(avgSpecList.Max());
            return fftX.Frequencies[maxIndex];
        }

      


    }
}
