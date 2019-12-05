using System;

using TremAn3.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TremAn3.Views
{
    public sealed partial class VideoInfoPage : Page
    {

        private VideoInfoViewModel ViewModel
        {
            get { return ViewModelLocator.Current.VideoInfoViewModel; }
        }

        public VideoInfoPage()
        {
            //this.DefaultStyleKey = typeof(VideoInfoPage);
        }
    }
}
