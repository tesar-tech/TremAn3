using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;

namespace TremAn3.ViewModels
{
    public  class ResultsViewModel : ViewModelBase
    {

        
        
        /// <summary>
        /// computes all the results and place it into the DataResutlsDict
        /// </summary>
        /// <param name="frameRate"></param>
        /// <param name="comXAllRois"></param>
        /// <param name="comYAllRois"></param>
        public async Task ComputeAllResults(double frameRate, List<List<double>> comXAllRois, List<List<double>> comYAllRois)
        {
            await Task.Run(() =>
            {
                Coherence coherence = new Coherence((int)frameRate, comXAllRois, comYAllRois);
                DataResultsDict.Clear();
                DataResultsDict.Add(DataSeriesType.Coherence, coherence.Compute());
            });
        }

        public Dictionary<DataSeriesType, DataResult> DataResultsDict { get; set; } = new Dictionary<DataSeriesType, DataResult>();

        //public Guid Id { get; set; } = Guid.Empty;


    }
}
