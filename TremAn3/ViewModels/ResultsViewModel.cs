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

        private double _CoherenceResult = 0;
        private IEnumerable<CenterOfMotionAlgorithm> comAlgs;

        public ResultsViewModel()
        {

        }
        public ResultsViewModel(IEnumerable<CenterOfMotionAlgorithm> comAlgs)
        {
            this.comAlgs = comAlgs;
        }

        public double CoherenceResult
        {
            get => _CoherenceResult;
            set => Set(ref _CoherenceResult, value);
        }

        public Guid Id { get; set; } = Guid.Empty;


    }
}
