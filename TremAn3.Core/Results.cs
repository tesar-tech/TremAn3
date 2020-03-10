using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TremAn3.Core
{
    public class Results
    {
        public List<double> listComX = new List<double>();
        public List<double> listComY = new List<double>();
        public List<(double x_freq, double y_power)> PsdAvgData { get; set; }
        public List<double> ListComXNoAvg
        {
            get
            {
                var avgX = listComX.Average();
                var noavg = listComX.Select(x => x - avgX).ToList();
                return noavg;
            }
        }

        public List<double> ListComYNoAvg
        {
            get
            {
                var avgY = listComY.Average();
                var noavg = listComY.Select(x => x - avgY).ToList();
                return noavg;
            }
        }
    }
}
