using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public class MeasurementModel
    {
        //only for json
        public MeasurementModel()
        {

        }
        public MeasurementModel(IEnumerable<CenterOfMotionAlgorithm> comAlgs)
        {
            Id = Guid.NewGuid();
            FrameRate = comAlgs.First().frameRate;//it is same for all the algs
            DateTime = DateTime.Now;
            foreach (var alg in comAlgs)
            {
                RoiResultModel alm = new RoiResultModel(alg.Results,alg.Rect);
                RoiResultModels.Add(alm);
            }
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public double Coherence { get; set; }
        public List<RoiResultModel> RoiResultModels { get; set; } = new List<RoiResultModel>();
        public double FrameRate { get; set; }
        public double Maxrange { get; set; }
        public double Minrange { get; set; }
        public double PositionSeconds { get; set; }
    }

    public class RoiResultModel
    {

        public RoiResultModel()
        {

        }
        public RoiResultModel(Results results, SelectionRectangle rect)
        {
            RoiModel = rect.RoiModel;
            ResultsModel = results.ResultsModel;
        }

        public RoiModel RoiModel { get; set; } = new RoiModel();
        public ResultsModel ResultsModel  { get; set; }



    }
    public class RoiModel
    {
        public Color Color { get; set; }
        public double SizeReductionFactor { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        
    }

    public class ResultsModel
    {
        public List<TimeSpan> FrameTimes { get; private set; } = new List<TimeSpan>();

        public List<double> ComX { get; private set; } = new List<double>();
        public List<double> ComY { get; private set; } = new List<double>();
        public List<double> FreqProgress { get;  set; } = new List<double>();
        public List<double> FreqProgressTime { get;  set; }
    }
}
