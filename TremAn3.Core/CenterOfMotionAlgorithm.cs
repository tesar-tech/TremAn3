using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TremAn3.Core.SignalProcessing;

namespace TremAn3.Core
{
    public class CenterOfMotionAlgorithm
    {
        public CenterOfMotionAlgorithm(int widthOfFrame, int heightOfFrame, double frameRate,SelectionRectangle rectangle)
        {
            if ( rectangle.IsZeroSum)
                rectangle.FullFromResolution(widthOfFrame, heightOfFrame);
            Rect = rectangle;
            this.widthOfFrame = (uint)widthOfFrame;

            this.frameRate = frameRate;
            previousValueX = (rectangle.Width - 1) / 2d;//previous is set to half in case of first and second frame are same
            previousValueY = (rectangle.Height - 1) / 2d;
            vecOfx = Enumerable.Repeat(Enumerable.Range(0, (int)rectangle.Width), (int)rectangle.Height).SelectMany(x => x).ToList();
            vecOfy = Enumerable.Range(0, (int)rectangle.Height).Select(x => Enumerable.Repeat(x, (int)rectangle.Width)).SelectMany(x => x).ToList();
            startInd = this.widthOfFrame * rectangle.Y + rectangle.X;
            endInd = startInd + (rectangle.Height - 1) * this.widthOfFrame + rectangle.Width - 1;
        }
        public CenterOfMotionAlgorithm(RoiResultModel rrm, double frameRate)
        {//creating by model (saved result)
         //Rect = new SelectionRectangle(rrm.RoiModel);//no need since rect are for UI only and
         //this whole thing is recreated affter pressin Computation button
            Results.ResultsModel = rrm.ResultsModel;
            this.frameRate = frameRate;
        }

        private uint widthOfFrame;

        //private uint heightOfFrame;
        /// <summary>
        /// start and end indexis of pixels in one frame (ROI)
        /// </summary>
        readonly uint startInd;
        readonly uint endInd;
        public SelectionRectangle Rect { get; private set; }
        //public byte[] Frame1 { get; set; }
        //public byte[] Frame2 { get; set; }
        public List<byte> Frame1 { get; set; }
        public List<byte> Frame2 { get; set; }

        //frame rate (of video originaly)
        readonly public double frameRate;

        readonly List<int> vecOfx;
        readonly List<int> vecOfy;

        //class where to save all results 
        public Results Results { get; set; } = new Results();

        double previousValueX;
        public double previousValueY;
        public double tdiffminmaxList = 0;
        public double tdiffminmaxArr = 0;
        public double tdiffnormmenList = 0;
        public double tdiffnormmenArr = 0;
        public double tcomxcomy = 0;
        public double tcomxcomyArr = 0;
        //Stopwatch sw = new Stopwatch();
        public void GetComFromCurrentARGBFrames()
        {
            double comX = previousValueX;//
            double comY = previousValueY;
            //sw.Restart();
            //var diff = Frame2.Zip(Frame1, (f2, f1) => (double)f2 - f1);

            //var max = diff.Max();
            //var min = diff.Min();
            //tdiffminmaxList += sw.ElapsedMilliseconds;
            //sw.Restart();

            var diffA = new double[Rect.Height*Rect.Width];
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
                for (uint c = 0; c < Rect.Width; c++)//iterates on single line
                {
                    int i = (int)(s + c) * 4;//index to RGBA Array
                    byte f1 = (byte)(0.2989 * Frame1[i] + 0.5870 * Frame1[i + 1] + 0.1140 * Frame1[i + 2]);//to gray
                    byte f2 = (byte)(0.2989 * Frame2[i] + 0.5870 * Frame2[i + 1] + 0.1140 * Frame2[i + 2]);

                    diffA[iter] = (double)f2 - f1;
                    //max and min computation
                    maxA = maxA < diffA[iter] ? diffA[iter] : maxA;//counting max in one loop
                    minA = minA > diffA[iter] ? diffA[iter] : minA;//counting min in one loop
                    iter++;
                }
            }

         

            //tdiffminmaxArr += sw.ElapsedMilliseconds;
            //sw.Restart();
            if (maxA != 0 || minA != 0)//frame 1 is different than frame 2
            {
                //sw.Restart();

                ////normalize frames
                //var diffNorm = diff.Select(x => (x - min) / (max - min));

                ////now "pixels" are in range from  0 to 1
                //var mean = diffNorm.Average();
                //tdiffnormmenList += sw.ElapsedMilliseconds;
                //sw.Restart();

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
                //tcomxcomyArr += sw.ElapsedMilliseconds;


            }
           Results.ResultsModel.ComX.Add(comX);
            Results.ResultsModel.ComY.Add(comY);
        }

        /// <summary>
        /// computes all other series from comX and comY
        /// </summary>
        public void ComputeAllData()
        {

            var comXNoAvg = Results.ResultsModel.ComX.RemoveAverage();
            var comYNoAvg = Results.ResultsModel.ComY.RemoveAverage();

            var frameTimes = Results.ResultsModel.FrameTimes.Select(x => x.TotalSeconds).ToList();
            //comx
            DataResult drComX = new DataResult()
            {
                X = frameTimes,
                Y = comXNoAvg
            };
            Results.DataResultsDict.Add(DataSeriesType.ComX, drComX);

            //comy
            DataResult drComY = new DataResult()
            {
                X = frameTimes,
                Y = comYNoAvg            };
            Results.DataResultsDict.Add(DataSeriesType.ComY, drComY);


            //psd
            var psdX = FreqAnalysis.Psd(comXNoAvg.ToArray(), frameRate);
            var psdY = FreqAnalysis.Psd(comYNoAvg.ToArray(), frameRate);

            DataResult drPsd = new DataResult()
            {
                X = SignalProcessingHelpers.GetFrequencies(psdX.Length, frameRate).ToList(),
                Y = psdX.Zip(psdY, (x, y) => (x + y) / 2).ToList()//do the average of x and y
            };
            Results.DataResultsDict.Add(DataSeriesType.Psd, drPsd);
            
            //ampspec
            var ampSpecX = Fft.AmpSpec(comXNoAvg.ToArray());
            var ampSpecY = Fft.AmpSpec(comYNoAvg.ToArray());
            DataResult dr = new DataResult()
            {
                X = SignalProcessingHelpers.GetFrequencies(ampSpecX.Length, frameRate).ToList(),
                Y = ampSpecX.Zip(ampSpecY, (x, y) => (x + y) / 2).ToList()//do the average of x and y
            };
            Results.DataResultsDict.Add(DataSeriesType.AmpSpec, dr);

            //welch
            var welchX = FreqAnalysis.Welch(comXNoAvg, frameRate);
            var welchY = FreqAnalysis.Welch(comYNoAvg, frameRate);

            DataResult drWelch = new DataResult()
            {
                X = SignalProcessingHelpers.GetFrequencies(welchX.Count(), frameRate).ToList(),
                Y = welchX.Zip(welchY, (x, y) => (x + y) / 2).Abs().ToList()//do the average of x and y
            };
            Results.DataResultsDict.Add(DataSeriesType.Welch, drWelch);

        }


        public void GetFftDuringSignal(int segmentSize, int step)
        {
            var fftProgressSignal =  FreqAnalysis.ComputeFftDuringSignalForTwoSignals(frameRate,Results.DataResultsDict[DataSeriesType.ComX].Y, Results.DataResultsDict[DataSeriesType.ComY].Y, segmentSize,step);
            if (fftProgressSignal.Count == 1)
                fftProgressSignal.Add(fftProgressSignal.First());

            Results.FreqProgress = fftProgressSignal;
            var firstTime = Results.ResultsModel.FrameTimes.First();
            var numberOfTicks = fftProgressSignal.Count;
            var segmentInSec = (Results.ResultsModel.FrameTimes.Last() - firstTime).TotalSeconds /(numberOfTicks-1);
            var range = Enumerable.Range(0, numberOfTicks).Select(x=>x*segmentInSec + firstTime.TotalSeconds).ToList();
            Results.FreqProgressTime = range;
        }
        

      


    }
}
