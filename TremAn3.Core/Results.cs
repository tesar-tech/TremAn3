using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public class Results
    {
        //public List<TimeSpan> FrameTimes { get; set; } = new List<TimeSpan>();

        //public List<double> listComX = new List<double>();
        //public List<double> listComY = new List<double>();
        public List<(double x_freq, double y_power)> PsdAvgData { get; set; }
        public List<double> ListComXNoAvg
        {
            get
            {
                var avgX = ResultsModel.ComX.Average();
                var noavg = ResultsModel.ComX.Select(x => x - avgX).ToList();
                return noavg;
            }
        }

        public List<double> ListComYNoAvg
        {
            get
            {
                var avgY = ResultsModel.ComY.Average();
                var noavg = ResultsModel.ComY.Select(x => x - avgY).ToList();
                return noavg;
            }
        }

        public ResultsModel ResultsModel { get;  set; } = new ResultsModel();


    }
}
