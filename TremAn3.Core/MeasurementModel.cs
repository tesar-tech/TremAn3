﻿using Newtonsoft.Json;
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

        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        //public double Coherence { get; set; }
        [JsonIgnore]
        public VectorsDataModel VectorsDataModel { get; set; }


        [JsonIgnore]
        public AdditionalResultsModel AdditionalResultsModel { get; set; } = new AdditionalResultsModel();

        public double FrameRate { get; set; }
        public double Maxrange { get; set; }
        public double Minrange { get; set; }
        public double PositionSeconds { get; set; }
        public int FreqProgressSegmnetSize { get; set; }
        public int FreqProgressStep { get; set; }

        public void GetResults(IEnumerable<CenterOfMotionAlgorithm> algs, Dictionary<DataSeriesType, DataResult> dataResultsDict, AdditionalResultsModel additionalResultsModel)
        {
            VectorsDataModel = new VectorsDataModel(algs, dataResultsDict);
            AdditionalResultsModel = AdditionalResultsModel;
        }




        //public int FftSegmentSize { get; set; }
    }

    public class VectorsDataModel
    {
        public VectorsDataModel() { }//for json
        public VectorsDataModel(IEnumerable<CenterOfMotionAlgorithm> comAlgs, Dictionary<DataSeriesType, DataResult> dataResultsDict)
        {
            foreach (var alg in comAlgs)
            {
                RoiResultModel alm = new RoiResultModel(alg.Results, alg.Rect);
                RoiResultModels.Add(alm);
            }

            foreach (var dr in dataResultsDict)
            {
                DataResultModel drm = new DataResultModel
                {
                    X = dr.Value.X,
                    Y = dr.Value.Y,
                    DataSeriesType = dr.Key,
                    ErrorMessage = dr.Value.ErrorMessage
                };
                GlobalScopedDataResultsModels.Add(drm);
            }
        }

        public List<RoiResultModel> RoiResultModels { get; set; } = new List<RoiResultModel>();

        public List<DataResultModel> GlobalScopedDataResultsModels { get; set; } = new List<DataResultModel>();


    }

    public class RoiResultModel
    {

        public RoiResultModel()
        {

        }
        public RoiResultModel(Results results, SelectionRectangle rect)
        {
            RoiModel = rect.RoiModel;

            foreach (var res in results.DataResultsDict)
            {
                DataResultModel r = new DataResultModel { X = res.Value.X, Y = res.Value.Y, DataSeriesType = res.Key, ErrorMessage = res.Value.ErrorMessage };
                DataResultsModels.Add(r);
            }
        }

        public RoiModel RoiModel { get; set; } = new RoiModel();
        //public ResultsModel ResultsModel  { get; set; }

        public List<DataResultModel> DataResultsModels { get; set; } = new List<DataResultModel>();



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

    public class DataResultModel
    {
        public List<double> X { get; set; }
        public List<double> Y { get; set; }
        public string ErrorMessage { get; set; }

        public DataSeriesType DataSeriesType { get; set; }
    }

    /// <summary>
    /// PSD is not saved here, cause it is computed from comx and comy...
    /// </summary>
    public class ResultsModel
    {
        public List<TimeSpan> FrameTimes { get; private set; } = new List<TimeSpan>();

        public List<double> ComX { get; private set; } = new List<double>();
        public List<double> ComY { get; private set; } = new List<double>();

    }


    public class AdditionalResultsModel
    {

        public List<CoherenceAverageResultModel> CoherenceAverageResults { get; set; } = new List<CoherenceAverageResultModel>();

        public List<PointsCollectorModel> PointsCollectors { get; set; } = new List<PointsCollectorModel>();

    }

    public class CoherenceAverageResultModel
    {
        public double MinHz { get; set; }
        public double MaxHz { get; set; }

        public double Average { get; set; }


    }


    public class PointsCollectorModel
    {
        public List<PointToCollectModel> Points { get; set; } = new List<PointToCollectModel>();

        public DataSeriesType DataSeriesType { get; set; }
    }

    public class PointToCollectModel
    {
        public double X { get; set; }
        public List< double> Ys { get; set; }
    }
}
