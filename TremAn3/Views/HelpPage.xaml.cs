using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TremAn3.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TremAn3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        //ViewLifetimeControl viewControl = null;
        public HelpPage()
        {
            this.InitializeComponent();
        }


        private ViewLifetimeControl _viewLifetimeControl;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
            _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        }

        private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        {
            _viewLifetimeControl.Released -= OnViewLifetimeControlReleased;
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                WindowManagerService.Current.SecondaryViews.Remove(_viewLifetimeControl);
            });
        }
        //bacause it can be pressed fast and twice
        bool _isCanRunContentDialog = true;
        private async void RunStartupGuide_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_isCanRunContentDialog)
            {
                _isCanRunContentDialog = false;
                FirstRunDialog fd = new FirstRunDialog(true);
                await fd.ShowAsync();
                _isCanRunContentDialog = true;

            }
        }
    }
}
