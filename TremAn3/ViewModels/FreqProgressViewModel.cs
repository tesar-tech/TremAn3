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
            set => Set(ref _Step, value);
        }


        public async Task FftLengthChanged()
        {
            ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.Generic;
            await FreqCounterViewModel.DisplayPlots(true, Core.DataSeriesType.FreqProgress);
            await FreqCounterViewModel.ParentVm.PastMeasurementsViewModel.SaveSelectedMeasurement();
            ViewModelLocator.Current.LoadingContentViewModel.Type = LoadingContentType.Off;
        }

        public async Task StepChanged()
        {
            await FftLengthChanged();//it is the same;
        }


        private int _SegmnetSize = 256;

        public int SegmnetSize
        {
            get => _SegmnetSize;
            set
            {
                Set(ref _SegmnetSize, value);
                    //FreqCounterViewModel.DisplayPlots(true, Core.DataSeriesType.FreqProgress).Wait();
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
