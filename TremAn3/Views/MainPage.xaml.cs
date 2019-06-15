using System;

using TremAn3.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TremAn3.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
