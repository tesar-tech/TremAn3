using System;
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

        public List<double> FreqProgress { get; set; } = new List<double>();
        public List<double> FreqProgressTime { get; set; }

        public ResultsModel ResultsModel { get;  set; } = new ResultsModel();

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
        Psd, ComX, ComY, AmpSpec, FreqProgress, Coherence,Welch
    }

}
