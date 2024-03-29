﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    /// <summary>
    /// here we have result that are computed from comX and comy, which is stored in ResultModel, which is saved to file
    /// thus, these results (like psd or freq progress) is computed on the fly (not stored in file)
    /// </summary>
    public class Results
    {
        //public List<double> FreqProgress { get; set; } = new List<double>();
        //public List<double> FreqProgressTime { get; set; }

        public void SetComXAndFrameTimes(List<double> comx, List<double> frametimes)
        {
            ComX = comx;
            FrameTimes = frametimes.Select(x => TimeSpan.FromSeconds(x)).ToList();
        }

        public void SetComY(List<double> comy)
        {
            ComY = comy;

        }


        public List<TimeSpan> FrameTimes { get; private set; } = new List<TimeSpan>();

        public List<double> ComX { get; private set; } = new List<double>();
        public List<double> ComY { get; private set; } = new List<double>();

        //public ResultsModel ResultsModel { get;  set; } = new ResultsModel();

        public Dictionary<DataSeriesType, DataResult> DataResultsDict { get; set; } = new Dictionary<DataSeriesType, DataResult>();
    }

    public class DataResult
    {
        //public DataSeriesType DataSeriesType{ get; set; }
        public List<double> X { get; set; }
        public List<double> Y { get; set; }

        public bool IsOk { get => string.IsNullOrWhiteSpace(ErrorMessage); }
        public string ErrorMessage { get; set; }
    }
    public enum DataSeriesType
    {
        Psd = 0, ComX = 1, ComY = 2, AmpSpec = 3, FreqProgress = 4, Coherence = 5, Welch = 6
    }
}
