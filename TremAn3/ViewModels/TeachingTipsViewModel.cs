using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TremAn3.Helpers;
using Windows.ApplicationModel.VoiceCommands;

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

        private bool _IsAlreadyRunTeachingTips = LocalSettings.Read(false, nameof(IsAlreadyRunTeachingTips));

        public bool IsAlreadyRunTeachingTips
        {
            get => _IsAlreadyRunTeachingTips;
            set
            {
               if( Set(ref _IsAlreadyRunTeachingTips, value))
                LocalSettings.Write(value);
            }
        }

        readonly Timer deferalStartTimer = new Timer();

        internal void StartIfAppropriate(double deferralSeconds = 0)
        {
            if (!IsAlreadyRunTeachingTips)
            {
                Start(deferralSeconds);
                IsAlreadyRunTeachingTips = true;
            }
        }

        internal void Start(double deferralSeconds = 0)
        {
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
            IsCountFreqTtipOpened = true;
        }

        private bool _IsCountFreqTtipOpened;

        public bool IsCountFreqTtipOpened
        {
            get => _IsCountFreqTtipOpened;
            set => Set(ref _IsCountFreqTtipOpened, value);
        }

        public void TtCountFreqClosed()
        {
            IsHelpTtipOpened = true;
        }
        private bool _IsHelpTtipOpened;

        public bool IsHelpTtipOpened
        {
            get => _IsHelpTtipOpened;
            set => Set(ref _IsHelpTtipOpened, value);
        }



    }
}
