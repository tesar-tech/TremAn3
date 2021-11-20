using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TremAn3.Core.SignalProcessing
{
    public class FreqAnalysis
    {
    

        public static IEnumerable<Complex32> mscohe(IEnumerable<double> signal1, IEnumerable<double> signal2, int windowLength, int segmentOverlapLength, double fs)
        {
            var cpsd = Welch(signal1, windowLength, segmentOverlapLength, fs, signal2);
            var welch1 = Welch(signal1, windowLength, segmentOverlapLength, fs);
            var welch2 = Welch(signal2, windowLength, segmentOverlapLength, fs);

            var welch1_welch2 = welch1.Zip(welch2, (one, two) => one * two);
            var cohe = cpsd.Select(x => x * x).Zip(welch1_welch2, (c, p) => c / p);
            return cohe;
        }

        public static IEnumerable<Complex32> Welch(IEnumerable<double> signal1, int windowLength, int segmentOverlapLength, double fs, IEnumerable<double> signal2 = null)
        {

            bool isCpsd = signal2 != null && signal2.Count() != 0;

            if (isCpsd && signal1.Count() != signal2.Count())
                throw new Exception("Data must be the same length");

            var segmentLength = windowLength;
            var data_length = signal1.Count();
            var window = NWaves.Windows.Window.Hamming(segmentLength);
            var windowCompensation = window.Select(x => x * x).Sum();
            int segmentCounter = 0;
            //fftlength is same as segmentLenght

            IEnumerable<Complex32> Power = new Complex32[segmentLength];


            for (int i = 0; i + segmentLength <= data_length; i += segmentLength - segmentOverlapLength)
            {
                var segment1 = Fft.fft(signal1.Skip(i).Take(segmentLength).Zip(window, (f, w) => w * f));

                if (isCpsd)
                {
                    var segment2 = Fft.fft(signal2.Skip(i).Take(segmentLength).Zip(window, (f, w) => w * f));
                    var seg1_seg2 = segment1.Zip(segment2, (one, two) => one * two.Conjugate());
                    Power = Power.Zip(seg1_seg2, (p, s) => p + s);
                }
                else
                    Power = Power.Zip(segment1, (p, s) => p + s * s.Conjugate());

                segmentCounter++;
            }

            var PowerMod = Power.Select(p => p / (segmentCounter * windowCompensation));
            float fsf = (float)fs;//for some reason I cannot divide complex number by double..
            var PowerHalf = PowerMod.Take(PowerMod.Count().HalfForPsd()).Select(x =>x/fsf);
            var PowerComplete = PowerHalf.Select((x, i) => (i > 0 && i < PowerHalf.Count() - 1) ? x * 2 : x);//all elements except first and last  multiply by 2
            return (IEnumerable<Complex32>) PowerComplete;
        }


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
            if (vector1.Count != vector2.Count)
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
            var frequencies = SignalProcessingHelpers.GetFrequencies(segment1.GetHalfForPsd().Count(), fs);
            do
            {
                vectorToBeCut1.CopyTo(0, segment1, 0, windowSizeSamples);//kde zacne v listu, kam to nakopiruje, kde zacne poklada, velikost
                vectorToBeCut1.RemoveRange(0, step);

                vectorToBeCut2.CopyTo(0, segment2, 0, windowSizeSamples);
                vectorToBeCut2.RemoveRange(0, step);

                //PsdResult res1 = GetAmpSpectrumAndMax(fs, segment1.ToList(), removeAvgInSegment);
                //PsdResult res2 = GetAmpSpectrumAndMax(fs, segment2.ToList(), removeAvgInSegment);

                var res1 = Fft.AmpSpec(segment1);
                var res2 = Fft.AmpSpec(segment2);

                //List<double> avgSpecList = new List<double>();
                //for (int i = 0; i < res1.Length; i++)
                //{
                //    double avg = (res1.Values[i] + res2.Values[i]) / 2;
                //    avgSpecList.Add(avg);
                //}

                var avgSpecList = res1.Zip(res2, (one, two) => (one + two) / 2);
                int maxIndex = avgSpecList.ToList().IndexOf(avgSpecList.Max());
                double maxFromSegment = frequencies[maxIndex];
                fftList.Add(maxFromSegment);
            }
            while (vectorToBeCut1.Count >= windowSizeSamples);

            return fftList;
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
        /// <summary>
        /// this function copy periodogram function from matlab. Does not use welch averaging. 
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static double[] Psd(double[] timeSeries, double fs)
        {
            var absfft = Fft.Abs_fft(timeSeries);
            var halfSpec = absfft.GetHalfForPsd();
            double fsRatioMult = (1.0 / (fs * absfft.Length));//1/(Fs*N))
            var psdx = halfSpec.Select(x => (fsRatioMult * x * x)).ToArray(); //(1/(Fs*N)) * abs(xdft).^2;  abs is already ther from abs_fft
            for (int i = 1; i < psdx.Length - 1; i++)
                psdx[i] *= 2; // psdx(2:end-1) = 2*psdx(2:end-1); multiply every (except 0 and fs/2) elem in spectrum by 2, to keep all the power from sepctrum (we discarder half previously)
            var psd = psdx.Select(x => (10 * Math.Log10(x))).ToArray();//10*log10(psdx);
            return psd;
            //this result is slightly different from matlab, but it is probably caused by
        }



    }
}
