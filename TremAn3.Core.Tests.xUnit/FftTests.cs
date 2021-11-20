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
        ////public FftTests()
        ////{ 
        ////}
        ///// <summary>
        ///// creates sin vector from time -10 to time 10s with expected sampling freq and frequency of sinus
        ///// </summary>
        ///// <param name="fs"></param>
        ///// <param name="frequency"></param>
        ///// <returns></returns>
        //private List<double> CreateVector(double fs, double frequency, double start, double stop, string signalType = "sin")
        //{
        //    List<double> vector = new List<double>();
        //    double xStep = 1 / fs;
        //    double y;
        //    if(signalType == "sin")
        //    {
        //        for (double xx = start; xx <= stop; xx += xStep)
        //        {
        //            y = Math.Sin(2 * Math.PI * frequency * xx);
        //            vector.Add(y);
        //        } 
        //    }
        //    else if(signalType == "sawtooth")
        //    {
        //        for (double xx = start; xx <= stop; xx += xStep)
        //        {
        //            y = 2 * (xx - Math.Floor(xx + 0.5));
        //            vector.Add(y);
        //        }
        //    }

        //    return vector;
        //}

        //[Fact]
        //public void GetAmpSpectrumAndMax_nullVector_returnsNull()
        //{
        //    var result = Fft.GetAmpSpectrumAndMax(100, null);
        //    Assert.Null(result);
        //}

        //[Fact]
        //public void GetAmpSpectrumAndMax_ShortVec_sameResult()
        //{
        //    List<double> Vector = CreateVector(1, 13,-10,10);
        //    PsdResult fft = new PsdResult();
        //    PsdResult result = Fft.GetAmpSpectrumAndMax(1, Vector);
        //    for (int i = 0; i < result.Values.Count; i++)
        //    {
        //        result.Values[i] *= 1e13;
        //        result.Values[i] = Math.Round(result.Values[i], 2, MidpointRounding.AwayFromZero);
        //    }
        //    List<(double, double)> valFreqTuple = new List<(double, double)>() {
        //        (0d,0d),(3.25,0.05),(1.49,0.1),(0.16,0.15),(1.36,0.2),(1.27,0.25),(0.03,0.3),(1.46,0.35),(1.92,0.4),(1.31,0.45),(0.54,0.5)
        //    };

        //    valFreqTuple.ForEach(x => { fft.Values.Add(x.Item1); fft.Frequencies.Add(x.Item2); });
        //    fft.MaxIndex = 1;

        //    Assert.Equal(result.Frequencies, fft.Frequencies);
        //    Assert.Equal(result.Values, fft.Values);
        //    Assert.Equal(result.MaxIndex, fft.MaxIndex);
        //    /*Hodnoty nejsou přesně shodné s matlabem(Matlab zaokrouhluje jinak než Math.Net)
        //      Math.Net počítá s větší přesností (e-21, matlab to zaokrouhlí na nulu)*/
        //}
        //[Fact]
        //public void GetAmpSpectrumAndMax_ShortVec1_sameResult()
        //{
        //    List<double> vecX = new List<double>() { 319.5, 319.570564104974, 319.570564104974, 319.387187167831 };
        //    List<double> vecY = new List<double>() { 239.5, 239.690534471355, 239.690534471355, 239.447377199610 };
        //    PsdResult resultX = Fft.GetAmpSpectrumAndMax(29.966, vecX);
        //    PsdResult resultY = Fft.GetAmpSpectrumAndMax(29.966, vecY);
        //    resultY.Values = resultY.Values.Select(x => Math.Round(x, 7)).ToList();
        //    resultX.Values = resultX.Values.Select(x => Math.Round(x, 7)).ToList();
        //    List<double> matlabValuesX = new List<double>() { 0, 0.1964851 };
        //    List<double> matlabValuesY = new List<double>() { 0, 0.3089156 };
        //    Assert.Equal(matlabValuesX, resultX.Values);
        //    Assert.Equal(matlabValuesY, resultY.Values);
        //}


        //[Fact]
        //public void GetAmpSpectrumAndMax_MultipleMaxIndexes_SameMaxIndexes()
        //{
        //    List<double> freq_matlab = new List<double>();
        //    List<double> freq_math_dotnet = new List<double>();
        //    double f;
        //    for (int i = 1; i <= 45; i++)
        //    {
        //        f = i * 20;
        //        freq_matlab.Add(f);
        //        List<double> Vector = CreateVector(100, i,-10,10);
        //        freq_math_dotnet.Add(Fft.GetAmpSpectrumAndMax(i, Vector).MaxIndex);
        //    }
        //    Assert.Equal(freq_matlab, freq_math_dotnet);
        //}
        //[Fact]
        //public void GetAmpSpectrumAndMax_EmptyInputList_returnsNull()
        //{
        //    List<double> vector = new List<double>();
        //    var result = Fft.GetAmpSpectrumAndMax(100, vector);
        //    Assert.Null(result);
        //}
        //[Fact]
        //public void GetAmpSpectrumAndMax_FsNotValid0_ThrowsError()
        //{
        //    var vector = CreateVector(100, 10,-10,10);
        //    Assert.Throws<ArgumentException>(() => Fft.GetAmpSpectrumAndMax(0, vector));
        //}
        //[Fact]
        //public void GetAmpSpectrumAndMax_FsNotValidLessThanZero_ThrowsError()
        //{
        //    var vector = CreateVector(100, 10,-10,10);
        //    Assert.Throws<ArgumentException>(() => Fft.GetAmpSpectrumAndMax(-10, vector));
        //}

        //[Fact]
        //public void ComputeFftDuringSignalForTwoSignals_BasicTests_GetCorectResult()
        //{
        //    double fs = 30;
        //    List<double> vector1 = Enumerable.Range(0,256).Select(x=>(double)x).ToList();
        //    List<double> vector2 = Enumerable.Range(5,256).Select(x=>(double)x).ToList();
        //    List<double> expected = new List<double>();
        //    List<double> vys = Fft.ComputeFftDuringSignalForTwoSignals(fs, vector1,vector2,256,1);
        //    for (int i = 0; i < 1040; i++)
        //    {
        //        expected.Add(7);
        //    }
        //    Assert.Equal(expected, vys);
        //}

        ////[Fact]//no time for tests...
        ////public void ComputeFftDuringSignal_SinSignal_sameResult()
        ////{
        ////    double fs = 35;
        ////    List<double> vector = CreateVector(fs, 7,-15,15);
        ////    List<double> expected = new List<double>();
        ////    List<double> vys = Fft.ComputeFftDuringSignal(fs, vector, 11, 1,false);
        ////    for (int i = 0; i < 1040;i++)
        ////    {
        ////        expected.Add(7);
        ////    }
        ////    Assert.Equal(expected, vys);
        ////}
        ////[Fact]
        ////public void ComputeFtDuringSignal_RealData_CorrectResult()
        ////{
        ////    var vX =new  List<double>(){ 319.5, 319.570564104974, 319.570564104974, 319.387187167831, 319.786477221865, 319.786477221865, 320.034733955658, 320.21267789075, 320.21267789075, 320.319014199821, 320.319014199821, 320.29565782616, 320.815334353301, 320.815334353301, 320.612711332284, 319.807712462201, 319.807712462201, 318.960858467838, 318.960858467838, 318.652020292339, 316.671365589703, 316.671365589703, 316.425042649623, 318.353972161642, 318.353972161642, 318.959306267319, 318.959306267319, 319.544982715054, 319.870415524594, 319.870415524594, 320.287392898376, 320.824542847701, 320.824542847701, 322.544139291768, 321.889293414764, 321.889293414764, 324.481623139042, 324.481623139042, 321.321487771761, 321.07848947741, 321.07848947741, 317.9513726414, 317.432122296376, 317.432122296376, 313.821065335645, 313.821065335645, 317.006312246112, 317.48114889544, 317.48114889544, 319.412232792966, 319.701524952699, 319.701524952699, 320.187712603404, 320.187712603404, 320.000473451935, 322.463595321388, 322.463595321388, 323.708728909394, 322.53232136517, 322.53232136517, 324.542649172811, 324.542649172811, 320.459189108305, 318.425697333715, 318.425697333715, 317.462973762777, 313.630602652244, 313.630602652244, 313.151720999448, 313.151720999448, 318.243139539219, 319.428634925258, 319.428634925258, 320.454046600434, 320.392041123662, 320.392041123662, 320.512573194874, 320.512573194874, 320.577462503779, 323.154595130694, 323.154595130694, 322.341800953875, 324.276436113765, 324.276436113765, 321.468294567702, 319.6426845466, 319.6426845466, 316.923222604587, 316.923222604587, 316.705561191695, 312.977268211447, 312.977268211447, 316.101630787472, 319.062219179602, 319.062219179602, 320.502973823378, 320.502973823378, 319.739140852584, 319.138812402459, 319.138812402459, 319.582347731823, 322.182191381445, 322.182191381445, 325.073140515567, 322.647099640661, 322.647099640661, 323.536744127976, 323.536744127976, 320.438184263498, 318.768556968945, 318.768556968945, 316.322901696976, 315.888421092885, 315.888421092885, 312.539661125865, 312.539661125865, 318.01016250079, 319.796520562963, 319.796520562963, 320.427363165301, 319.335549996678, 319.335549996678, 319.298584155443, 319.298584155443, 320.559226334133, 323.504423164824, 323.504423164824, 325.158692137796, 321.090630286618, 321.090630286618, 319.993228822078, 319.993228822078, 318.32615022036, 315.429340170577, 315.429340170577, 316.755794828093, 315.55130588702, 315.55130588702, 319.372105880498, 321.023469770846, 321.023469770846, 320.693029564719, 320.693029564719, 319.178263444903, 317.863930547177, 317.863930547177, 321.697275514516, 323.185717971381, 323.185717971381, 327.039851195977, 327.039851195977, 321.765497186018, 321.196242249577, 321.196242249577, 317.584083939386, 317.572078015152, 317.572078015152, 313.413936158419, 313.413936158419, 315.808422780836, 316.666833391663, 316.666833391663, 319.772067558723, 323.181420966446, 323.181420966446, 317.899888740212, 317.829058690112, 317.829058690112, 320.492594945849, 320.492594945849, 323.365334071827, 329.279900313933, 329.279900313933, 324.852725256295, 320.802832417952, 320.802832417952, 317.340325125624, 317.340325125624, 317.202305878905, 313.113682083011, 313.113682083011, 311.650388000807, 318.164817142206, 318.164817142206, 321.85229704793, 321.85229704793, 322.087221316055, 321.527417045856, 321.527417045856, 315.744625841215, 317.800171503788, 317.800171503788, 321.62168977973, 321.62168977973, 323.386101373754, 328.395622489687, 328.395622489687, 322.841532698844, 323.285701120305, 323.285701120305, 317.965854870868, 317.028309804576, 317.028309804576, 314.571902931938, 314.571902931938, 315.065219715537, 315.286166691966, 315.286166691966, 319.939522686362, 320.902835036735, 320.902835036735, 321.54372212642, 321.54372212642, 319.030726220437, 317.00343413315, 317.00343413315, 319.955844716289, 319.955844716289, 321.918684214233, 325.954495615337, 325.954495615337, 323.171408518884, 324.452460548107, 324.452460548107, 321.581366241851, 318.956808697365, 318.956808697365, 316.669521216198, 316.669521216198, 317.07413897253, 313.446631982503, 313.446631982503, 316.381064283574, 316.919951057578, 316.919951057578, 320.811664897789, 320.811664897789, 321.564832620253, 322.269845759589, 322.269845759589, 320.512635236787, 318.263712470154, 318.263712470154, 316.455113788133, 318.415753296468, 318.415753296468, 322.521395995591, 322.521395995591, 323.044411352892, 324.12072469397, 324.12072469397, 318.363312585857, 317.705762400722, 317.705762400722, 316.893179443906, 316.893179443906, 319.416295135371, 321.496962323897, 321.496962323897, 321.772508671891, 319.90003156916, 319.90003156916, 316.991609521648, 316.991609521648, 317.950417602287, 315.370746986205, 315.370746986205, 314.742654441894, 333.014463509682, 333.014463509682, 315.74727910766, 317.778231793732, 317.778231793732, 315.46543513969, 315.46543513969, 319.470497727261, 318.042236155327, 318.042236155327, 319.287658019028, 319.730248666031, 319.730248666031, 316.700127126166, 316.700127126166, 320.024784919234};
        ////    var fs = 29.966;
        ////    var window = 200;
        ////    var avg = vX.Average();
        ////    var withoutAvg = vX.Select(x => x - avg).ToList();
        ////    List<double> vys = Fft.ComputeFftDuringSignal(fs, withoutAvg, window, 5, false);
        ////    Random rnd = new Random();
        ////    for (int i = 0; i < 10;i++)
        ////    {
        ////        var vysIndex = rnd.Next(0, vys.Count-1);// generate random index for comparison
        ////        Assert.Equal(1.3620909091, vys[vysIndex]);
        ////    }
        ////}

        ////[Fact]
        ////public void ComputeFftDuringSignal_SawToothSignal_sameResult()
        ////{
        ////    double fs = 30;
        ////    List<double> vector = CreateVector(fs, 5, -10, 10, "sawtooth");
        ////    List<double> expected = new List<double>();
        ////    List<double> vys = Fft.ComputeFftDuringSignal(fs, vector, 8, 1);
        ////    var result = vys.Select(x => Math.Round(x, 4)).ToList();
        ////    for (int i = 0; i < 593; i++)
        ////    {
        ////        expected.Add(5);
        ////    }
        ////    Assert.Equal(expected, result);
        ////}

    }
}