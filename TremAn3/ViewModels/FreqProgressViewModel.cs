using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
  public  class FreqProgressViewModel:ViewModelBase
    {

        public FreqCounterViewModel FreqCounterViewModel { get => ViewModelLocator.Current.FreqCounterViewModel; }

        private int _Step = 2;

        public int Step
        {
            get => _Step;
            set
            {
                if (Set(ref _Step, value))
                    FreqCounterViewModel.ReDrawFreqProgress(true);
            }
        }

        private int _SegmnetSize = 256;

        public int SegmnetSize
        {
            get => _SegmnetSize;
            set
            {
                if (Set(ref _SegmnetSize, value))
                    FreqCounterViewModel.ReDrawFreqProgress(true);
            }
        }

        private string _StatusMessage;

        public string StatusMessage
        {
            get => _StatusMessage;
            set => Set(ref _StatusMessage, value);
        }

        private bool _IsFreqProgressParametersOk = true;

        public bool IsFreqProgressParametersOk
        {
            get => _IsFreqProgressParametersOk;
            set => Set(ref _IsFreqProgressParametersOk, value);
        }



    }
}
