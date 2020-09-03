using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TremAn3.ViewModels
{
    public class TeachingTipsViewModel : ViewModelBase
    {

        private bool _IsRoiTtipOpened;

        public bool IsRoiTtipOpened
        {
            get => _IsRoiTtipOpened;
            set => Set(ref _IsRoiTtipOpened, value);
        }

        readonly Timer deferalStartTimer = new Timer();

        internal void Start(double deferralSeconds = 0)
        {
            return;
            if (deferralSeconds == 0)
                IsRoiTtipOpened = true;
            else
            {  //we need to deferal display of teching tip.
                //It's bcs of grid spliter and "true" size of drawing rectangle is made, after the load.
                //thus it displayed the tt not in the correct position. 
                deferalStartTimer.Interval = deferralSeconds * 1000;
                deferalStartTimer.Elapsed += async (object sender, ElapsedEventArgs e) =>
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Start(0);
                    });
                    deferalStartTimer.Stop();
                };
                deferalStartTimer.Start();
            }
        }

        public void TtRoiClosed()
        {
            IsMediaRangeSelectorTtipOpened = true;
        }


        private bool _IsMediaRangeSelectorTtipOpened;

        public bool IsMediaRangeSelectorTtipOpened
        {
            get => _IsMediaRangeSelectorTtipOpened;
            set => Set(ref _IsMediaRangeSelectorTtipOpened, value);
        }

        public void TtRangeSelectorClosed()
        {

        }

    }
}
