using System;
using System.Collections.Generic;
using System.Text;

namespace TremAn3.Core
{

    public interface ParameterToCompute<T>
    {
        T Compute();
    }

   public class Coherence:ParameterToCompute<double>
    {
        public Coherence()
        {

        }
        public List<double[]> ComXAllRois { get; set; } = new List<double[]>();
        public List<double[]> ComYAllRois { get; set; } = new List<double[]>();

        public double Compute()
        {
            return 123;
        }
    }

}
