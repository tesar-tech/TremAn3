using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TremAn3.Core.SignalProcessing;

namespace TremAn3.Core
{

    public interface ResultToCompute<T>
    {
        T Compute();
    }

    public class Coherence : ResultToCompute<DataResult>
    {
        public Coherence(double frameRate, List<List<double>> comXAllRois, List<List<double>> comYAllRois)
        {
            this.frameRate = frameRate;
            this.ComXAllRois = comXAllRois;
            this.ComYAllRois = comYAllRois;

        }
        double frameRate;
        private List<List<double>> ComXAllRois { get; set; }
        private List<List<double>> ComYAllRois { get; set; }

        public DataResult Compute()
        {
            if (ComXAllRois.Count < 2 || ComYAllRois.Count < 2)
                return new DataResult() { ErrorMessage = "For Coherence computation two ROIs are necessary!!" };
            var w = 256; var ov = w - 1;
            var a_x = ComXAllRois.First().ToArray();
            var a_y = ComYAllRois.First().ToArray();
            var aa = a_x.Zip(a_y, (x, y) => Math.Sqrt(x * x + y * y));//vectors

            var b_x = ComXAllRois[1].ToArray();
            var b_y = ComYAllRois[1].ToArray();
            var bb = b_x.Zip(b_y, (x, y) => Math.Sqrt(x * x + y * y));

            var dr = new DataResult();
            try
            {
                var cohe = FreqAnalysis.mscohe(aa, bb, w, ov, frameRate).Select(x => x.Magnitude);

                dr.Y = cohe.ToD().ToList();
                dr.X = SignalProcessingHelpers.GetFrequencies(cohe.Count(), frameRate).ToList();
            }
            catch (Exception ex)
            {
                dr.ErrorMessage = ex.Message;
            }

            return dr;

        }
    }





}
