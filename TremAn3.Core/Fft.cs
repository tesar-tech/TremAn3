﻿using System;
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
        //public static PsdResult GetAmpSpectrumAndMax(double fs, List<double> vector,bool removeAverage = true)
        //{
        //    if (vector == null || !vector.Any())
        //        return null;
        //    if (vector.Count == 1)
        //        throw new ArgumentException("Vector must have more than 1 value",nameof(vector));
        //    if (fs <= 0)
        //        throw new ArgumentException("Fs cannot be less than or equal to zero", nameof(fs));
                
        //    Complex32[] vec = new Complex32[vector.Count];
        //    int i = 0;
        //    double vecAvg = removeAverage ? vector.Average():0;//don't compute avg if not needed
        //    foreach (var ve in vector)
        //    {
        //        vec[i] = removeAverage ? new Complex32((float)(ve - vecAvg), 0) : new Complex32((float)ve, 0);
        //        i++;
        //    }
        //    Fourier.Forward(vec, FourierOptions.Matlab);

        //    PsdResult res = new PsdResult();
        //    res.Frequencies = new List<double>();
        //    res.Values = new List<double>();
        //    int l = (int)Math.Round((double)vec.Length / 2, MidpointRounding.AwayFromZero);
        //    double konec = fs / 2;
        //    var k = Generate.LinearSpaced(l, 0, konec);
        //    for (int p = 0; p < l; p++)
        //    {
        //        k[p] = Math.Round(k[p], 10);
        //        res.Frequencies.Add(k[p]);
        //        res.Values.Add(Complex32.Abs(vec[p]));
        //    }
        //    res.MaxIndex = res.Values.FindIndex(n => n == (res.Values.Max()));
        //    return res;
        //}
        //public static PsdResult GetPsdAndMax(double fs, List<double> vector, bool removeAverage = true)
        //{
        //    PsdResult fftRes = GetAmpSpectrumAndMax(fs, vector, removeAverage);
        //    fftRes.Values.ForEach(v => v *= v);//psad is amp spec^2
        //    return fftRes;
        //}
       




        public static List<double> ComputeFftDuringSignalForTwoSignals(double fs, List<double> vector1, List<double> vector2, int windowSizeSamples, int step)
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
                throw new WindowLowerThanOneException();
            if (step <= 0)
                throw new StepLowerThanOneException();
            if(vector1.Count != vector2.Count)
                throw new ArgumentException("Vectors have to be same length", $"{nameof(vector1)},{nameof(vector2)}");
            if (vector1.Count < windowSizeSamples)
                throw new WindowLongerThanSignalException(windowSizeSamples, vector1.Count);


            //if(windowSize + step > vector.Count)
            //    throw new ArgumentException("WindowSize + step must be smaller than count of values in vector", nameof(step));

            var vectorToBeCut1 = new List<double>(vector1);//copy list 
            var vectorToBeCut2 = new List<double>(vector2);//copy list 
            var fftList = new List<double>();
            double[] segment1 = new double[windowSizeSamples];
            double[] segment2 = new double[windowSizeSamples];
            var frequencies = GetFrequencies(segment1.GetHalf().Length, fs);
            do
            {
                vectorToBeCut1.CopyTo(0, segment1, 0, windowSizeSamples);//kde zacne v listu, kam to nakopiruje, kde zacne poklada, velikost
                vectorToBeCut1.RemoveRange(0, step);

                vectorToBeCut2.CopyTo(0, segment2, 0, windowSizeSamples);
                vectorToBeCut2.RemoveRange(0, step);

                //PsdResult res1 = GetAmpSpectrumAndMax(fs, segment1.ToList(), removeAvgInSegment);
                //PsdResult res2 = GetAmpSpectrumAndMax(fs, segment2.ToList(), removeAvgInSegment);

                var res1 = Abs_fft(segment1).GetHalf();
                var res2 = Abs_fft(segment2).GetHalf();

                //List<double> avgSpecList = new List<double>();
                //for (int i = 0; i < res1.Length; i++)
                //{
                //    double avg = (res1.Values[i] + res2.Values[i]) / 2;
                //    avgSpecList.Add(avg);
                //}

                var avgSpecList =  res1.Zip(res2, (one, two) => (one + two) / 2);
                int maxIndex = avgSpecList.ToList().IndexOf(avgSpecList.Max());
                double maxFromSegment = frequencies[maxIndex];
                fftList.Add(maxFromSegment);
            }
            while (vectorToBeCut1.Count >= windowSizeSamples);
           
            return fftList;
        }

        //abs(fft(v))
        //in: [0.022159,0.022159,-0.088309,-0.111715,0.104101,0.357491,0.13677,-0.388899,-1.574828,-2.952296,-1.042132,1.074979,1.273232,1.645892,0.940059,0.475872,-0.251514,-0.070993,0.241776,-0.142496,-0.998711,-2.168823,2.235159,0.969833,0.044245,0.598955,-1.23198,0.023755,0.037551,0.169932,0.165963,0.189136,1.187113,-0.284311,-3.204018,2.283397,0.888993,1.452861,-0.692839,-0.446676,0.470987,-0.175029,-0.153575,2.364288,-2.54245,0.075023,-2.399834,4.265676,-0.861218,0.229128,-0.290234,0.075764,0.082581,0.003762,1.413682,-1.862601,-1.062024,0.596845,1.671393,1.192122,0.401572,-0.481183,2.005393,0.453755,-1.309015,-0.894265,2.026556,-3.558884,0.82854,-3.855915,5.196975,0.090063,-0.24161,-0.736389,0.134124,0.043472,0.716045,-0.583951,0.874849,-1.893072,1.726022,0.44529,1.665646,-1.252369,1.599883,0.716631,0.75279,0.124685,-2.029274,0.759499,-1.332234,0.392441,-3.681886,4.921633,-0.237671,0.108195,-0.338458,0.061479,0.029136,-0.002236,1.669553,-1.423428,-2.846713,2.608221,0.464307,3.652914,-1.876057,2.530471,0.305525,-1.310979,0.944181,-0.372085,-3.111038,1.330501,-1.620883,-3.50324,5.437183,0.246148,-1.860588,0.050095,0.128776,-0.004455,0.408427,-0.598351,1.074352,-3.083693,2.290285,-0.108997,3.563782,-2.603226,0.012996,-0.06797,0.675457,1.920409,-4.23345,2.020022,-3.679586,1.867735,0.6516,3.349744,-2.667903,0.228167,0.088098,0.002343,-0.039499,0.545734,-1.423603,-3.423991,3.592438,1.143935,0.403595,1.814296,2.205879,-0.285674,0.220108,-0.168733,-2.808349,1.069739,-3.183721,0.821209,-3.566106,4.476038,2.336886,-1.379269,0.24929,0.092704,-0.075929,-0.274845,1.645068,-1.385275,-1.732164,0.145691,3.131479,1.803745,-0.016623,-6.138647,-0.109619,2.79834,1.807304,-4.462644,0.935302,-0.442884,-4.789844,5.125612,1.410191,-2.960793,0.522179,0.207242,0.088191,-0.062683,-0.098262,0.448746,-0.995293,-4.650503,5.283585,0.737501,-0.226252,4.002884,-1.65487,-8.002618,-0.621409,1.625541,3.298843,-3.206325,0.574953,-4.763033,0.673709,-0.91195,2.168487,3.223045,-1.600577,0.204563,0.064811,0.014276,-0.018664,-0.062511,1.271683,-0.446584,-4.443106,4.08895,0.59547,0.636716,2.261967,0.7565,-5.28708,-5.340505,0.20584,1.529248,2.976036,-2.89616,0.314244,-3.706493,1.23063,-3.6589,5.051797,-0.213443,0.465522,-0.154222,0.211709,0.08949,0.048761,0.028273,0.089276,-0.036518,1.802129,-2.134179,-3.384178,4.113419,-1.98383,3.64653,-3.135545,1.263758,-0.521326,-1.720668,-0.237182,0.529593,4.565157,2.827514,-0.373911,-1.837989,-1.066692,-0.310473,0.160214,0.927439,2.21425,3.724252,-0.451506,-11.19445,-7.451156,-4.853394,6.572779,1.902971,4.410998,6.409877,3.018027,0.919924,1.323119,0.681247,0.273065,-2.088025,-3.226057,-1.477885,0.07637,-0.750098,-1.433567,-2.927478]
        //out: [0.022162914,17.787664,5.0962234,14.222686,7.3371835,15.381386,6.604598,4.7970266,9.516452,19.973967,27.204895,11.047341,12.306418,52.829517,32.700363,32.65028,36.852055,32.66595,30.769463,40.923992,29.305267,52.779648,41.325542,42.665974,36.15743,64.20372,46.88661,16.836163,26.219925,20.4398,21.539833,51.311527,55.282314,68.34193,47.784283,13.874551,31.920998,46.55825,44.783077,42.491962,58.022636,58.044605,54.408752,37.06483,22.83835,62.65121,67.29082,45.221428,35.793903,11.26062,89.68795,43.71373,44.665222,24.936054,36.13238,25.204437,14.335369,46.72153,35.22503,47.990814,18.058775,12.969578,43.8534,19.867496,25.90793,42.027393,48.315907,40.217793,33.28899,45.274483,12.817056,27.843899,12.812805,42.289024,36.285103,40.45841,39.03753,13.583561,45.003914,40.110317,61.932858,71.25824,33.054672,28.165947,14.24375,30.61582,32.805176,31.026533,53.950066,61.954906,45.51912,10.770975,19.637959,27.853815,22.351406,39.545868,14.252341,9.540558,35.661278,47.97148,27.696796,25.385832,50.94747,23.008934,19.183447,32.06221,55.773354,33.957756,26.583647,19.46434,2.6687748,17.852297,23.861687,64.257645,40.22731,33.728806,23.951084,7.6542816,25.521683,18.826534,48.118713,57.158005,45.408257,50.37392,46.668427,53.145294,23.97181,21.86185,15.143555,38.435116,49.260136,35.437817,55.208847,76.87582,42.43742,50.33384,67.21707,50.28931,64.27204,49.66942,31.415337,31.70991,30.796099,25.82425,30.79609,31.709911,31.41534,49.669422,64.27204,50.289303,67.21707,50.333843,42.437416,76.87582,55.208843,35.437824,49.26014,38.435112,15.143562,21.861841,23.971817,53.145294,46.66843,50.373928,45.408253,57.158,48.11872,18.826544,25.521685,7.6542754,23.951075,33.72881,40.227314,64.25763,23.861681,17.852291,2.6687727,19.464334,26.58365,33.957745,55.773354,32.06222,19.183447,23.00894,50.947475,25.385826,27.6968,47.97148,35.66128,9.540554,14.25234,39.545876,22.351408,27.853802,19.637964,10.770969,45.519123,61.954906,53.950066,31.02653,32.805176,30.615824,14.243752,28.16595,33.054672,71.25823,61.932842,40.110317,45.003918,13.583553,39.03752,40.45841,36.2851,42.28903,12.812806,27.843895,12.817065,45.274487,33.28899,40.217793,48.315907,42.0274,25.907925,19.8675,43.8534,12.969576,18.058773,47.990814,35.225033,46.72154,14.33537,25.204435,36.13238,24.936054,44.66522,43.71373,89.68795,11.260626,35.7939,45.22144,67.29082,62.65121,22.838345,37.064842,54.408752,58.044605,58.02263,42.491947,44.783077,46.558254,31.920992,13.874548,47.784275,68.34193,55.28231,51.311527,21.53983,20.439796,26.219921,16.836159,46.88661,64.20373,36.157417,42.66596,41.325542,52.77965,29.305267,40.923996,30.769474,32.66595,36.852055,32.65027,32.70037,52.829525,12.306415,11.047343,27.204891,19.97397,9.516448,4.7970295,6.6045985,15.381387,7.337184,14.222681,5.0962195,17.78766]
        //this is basically amp spectrum
        private static double[] Abs_fft(double[] vector)
        {
            // Complex32[] vec = new Complex32[vector.Length];
            var complexVector = vector.Select(x => new Complex32((float)x, 0)).ToArray();
            Fourier.Forward(complexVector, FourierOptions.Matlab);
            var abs_value = complexVector.Select(x => (double)x.Magnitude).ToArray();
            return abs_value;
        }

        public static double[] AmpSpec(double[] vector)
        {
            return  Abs_fft(vector).GetHalf().ToArray();
        }


        //vector = vector(1:N/2+1);
        //in: [1 2 3 4 5] out: [1 2 3]
        //in: [1 2 3 4 5 6] out: [1 2 3 4]
        // float [] ar = new float []{1,2,3,4,5};
        // ar.GetHalfForPsd() 
        private static T[] GetHalf<T>(this T[] vector)
        {
            int l = (int)Math.Round(vector.Length / 2.0, MidpointRounding.ToEven) + 1;
            return vector.Take(l).ToArray();
        }
        //in: [0.022159,0.022159,-0.088309,-0.111715,0.104101,0.357491,0.13677,-0.388899,-1.574828,-2.952296,-1.042132,1.074979,1.273232,1.645892,0.940059,0.475872,-0.251514,-0.070993,0.241776,-0.142496,-0.998711,-2.168823,2.235159,0.969833,0.044245,0.598955,-1.23198,0.023755,0.037551,0.169932,0.165963,0.189136,1.187113,-0.284311,-3.204018,2.283397,0.888993,1.452861,-0.692839,-0.446676,0.470987,-0.175029,-0.153575,2.364288,-2.54245,0.075023,-2.399834,4.265676,-0.861218,0.229128,-0.290234,0.075764,0.082581,0.003762,1.413682,-1.862601,-1.062024,0.596845,1.671393,1.192122,0.401572,-0.481183,2.005393,0.453755,-1.309015,-0.894265,2.026556,-3.558884,0.82854,-3.855915,5.196975,0.090063,-0.24161,-0.736389,0.134124,0.043472,0.716045,-0.583951,0.874849,-1.893072,1.726022,0.44529,1.665646,-1.252369,1.599883,0.716631,0.75279,0.124685,-2.029274,0.759499,-1.332234,0.392441,-3.681886,4.921633,-0.237671,0.108195,-0.338458,0.061479,0.029136,-0.002236,1.669553,-1.423428,-2.846713,2.608221,0.464307,3.652914,-1.876057,2.530471,0.305525,-1.310979,0.944181,-0.372085,-3.111038,1.330501,-1.620883,-3.50324,5.437183,0.246148,-1.860588,0.050095,0.128776,-0.004455,0.408427,-0.598351,1.074352,-3.083693,2.290285,-0.108997,3.563782,-2.603226,0.012996,-0.06797,0.675457,1.920409,-4.23345,2.020022,-3.679586,1.867735,0.6516,3.349744,-2.667903,0.228167,0.088098,0.002343,-0.039499,0.545734,-1.423603,-3.423991,3.592438,1.143935,0.403595,1.814296,2.205879,-0.285674,0.220108,-0.168733,-2.808349,1.069739,-3.183721,0.821209,-3.566106,4.476038,2.336886,-1.379269,0.24929,0.092704,-0.075929,-0.274845,1.645068,-1.385275,-1.732164,0.145691,3.131479,1.803745,-0.016623,-6.138647,-0.109619,2.79834,1.807304,-4.462644,0.935302,-0.442884,-4.789844,5.125612,1.410191,-2.960793,0.522179,0.207242,0.088191,-0.062683,-0.098262,0.448746,-0.995293,-4.650503,5.283585,0.737501,-0.226252,4.002884,-1.65487,-8.002618,-0.621409,1.625541,3.298843,-3.206325,0.574953,-4.763033,0.673709,-0.91195,2.168487,3.223045,-1.600577,0.204563,0.064811,0.014276,-0.018664,-0.062511,1.271683,-0.446584,-4.443106,4.08895,0.59547,0.636716,2.261967,0.7565,-5.28708,-5.340505,0.20584,1.529248,2.976036,-2.89616,0.314244,-3.706493,1.23063,-3.6589,5.051797,-0.213443,0.465522,-0.154222,0.211709,0.08949,0.048761,0.028273,0.089276,-0.036518,1.802129,-2.134179,-3.384178,4.113419,-1.98383,3.64653,-3.135545,1.263758,-0.521326,-1.720668,-0.237182,0.529593,4.565157,2.827514,-0.373911,-1.837989,-1.066692,-0.310473,0.160214,0.927439,2.21425,3.724252,-0.451506,-11.19445,-7.451156,-4.853394,6.572779,1.902971,4.410998,6.409877,3.018027,0.919924,1.323119,0.681247,0.273065,-2.088025,-3.226057,-1.477885,0.07637,-0.750098,-1.433567,-2.927478];
        //out: [-72.39329,-11.293144,-22.150555,-13.23589,-18.984936,-12.5556135,-19.898596,-22.67608,-16.726023,-10.286237,-7.602582,-15.430367,-14.492889,-1.8379903,-6.0044713,-6.017785,-4.966289,-6.013617,-6.5331244,-4.055963,-6.9566092,-1.8461934,-3.9711518,-3.6938899,-5.1315722,-0.14431909,-2.8745468,-11.77066,-7.922894,-10.08599,-9.630676,-2.0912244,-1.4437987,0.39822158,-2.7098215,-13.451144,-6.2139935,-2.93559,-3.2732444,-3.7293873,-1.023574,-1.0202858,-1.5821477,-4.9162827,-9.122229,-0.35693377,0.26359293,-3.1886375,-5.2193418,-15.2642765,2.759159,-3.4831657,-3.296133,-8.358968,-5.1375914,-8.265983,-13.167345,-2.905182,-5.3584957,-2.6723607,-11.161757,-14.037006,-3.4554574,-10.33266,-8.026869,-3.824874,-2.6137202,-4.2071586,-5.8495107,-3.1784532,-14.139758,-7.4009223,-14.142638,-3.7709696,-5.100956,-4.1553473,-4.4658766,-13.63525,-3.2305171,-4.230401,-0.45710075,0.7611788,-5.910866,-7.301036,-13.223037,-6.5766053,-5.976676,-6.4608583,-1.6556836,-0.45400882,-3.131646,-15.650423,-10.433597,-7.3978295,-9.309426,-4.353501,-13.217798,-16.704048,-5.251585,-2.6758604,-7.4469323,-8.203695,-2.1530704,-9.057593,-10.636991,-6.175654,-1.366988,-5.676743,-7.803232,-10.51073,-27.769285,-11.2616415,-8.7415,-0.13702706,-4.205103,-5.7355037,-8.70902,-18.617434,-8.157337,-10.800116,-2.6492429,-1.1539819,-3.1528265,-2.251408,-2.9150598,-1.7862269,-8.701507,-9.501784,-12.690967,-4.6009593,-2.4456112,-5.306184,-1.4553496,1.4202718,-3.7405436,-2.258322,0.25406882,-2.2660093,-0.13508125,-2.3737416,-6.352689,-6.2716227,-6.525609,-11.0652685];
        //x = vector;
        // N = length(x); x is timeseries
        // xdft = fft(x);
        // xdft = xdft(1:N/2+1);
        // psdx = (1/(Fs*N)) * abs(xdft).^2;
        // psdx(2:end-1) = 2*psdx(2:end-1)
        //psd = 10*log10(psdx);
        public static double[] Psd(double[] timeSeries, double fs)
        {
            var absfft = Abs_fft(timeSeries);
            var halfSpec = absfft.GetHalf();
            double fsRatioMult = (1.0 / (fs * absfft.Length));//1/(Fs*N))
            var psdx = halfSpec.Select(x => (fsRatioMult * x * x)).ToArray(); //(1/(Fs*N)) * abs(xdft).^2;  abs is already ther from abs_fft
            for (int i = 1; i < psdx.Length - 1; i++)
                psdx[i] *= 2; // psdx(2:end-1) = 2*psdx(2:end-1); multiply every (except 0 and fs/2) elem in spectrum by 2, to keep all the power from sepctrum (we discarder half previously)
            var psd = psdx.Select(x => (10 * Math.Log10(x))).ToArray();//10*log10(psdx);
            return psd;
            //this result is slightly different from matlab, but it is probably caused by
        }

        //freq = 0:Fs/length(x):Fs/2;
        //this is bit different from maltab since freq(end) is not fs/2 for odd elements in matlab
        public static double[] GetFrequencies(int count, double fs)
        {
            var k = Generate.LinearSpaced(count, 0, fs/2).ToArray();
            return k;
        }

        public static List<double> RemoveAverage(this List<double> array) 
        {
            var average = array.Average();
            var woutAvg  = array.Select(x => x - average).ToList();
            return woutAvg;

        }

    }

    public class AmpSpecResult
    {
        public List<double> Frequencies { get; set; } = new List<double>();
        public List<double> Values { get; set; } = new List<double>();
    }

    public class PsdResult
    {
        //public int MaxIndex { get; set; }
        public List<double> Frequencies { get; set; } = new List<double>();
        public List<double> Values { get; set; } = new List<double>();
    }
}
