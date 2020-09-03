using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TremAn3.Views
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog(bool lightDismiss = false)
        {
            this.lightDismiss = lightDismiss;
            // TODO WTS: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex += 1;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex -= 1;

        }

        private void FlipView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (FlipView.SelectedIndex < FlipView.Items.Count - 1)
            {
                FlipView.SelectedIndex += 1;

            }
        }

        bool isFirstRun = true;
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isFirstRun)
            {
                isFirstRun = false;
                return;
            }
            BtnPrevious.Visibility = FlipView.SelectedIndex == 0 ? Visibility.Collapsed : Visibility.Visible;

            if (FlipView.SelectedIndex == FlipView.Items.Count - 1)
            {
                BtnNext.Visibility = Visibility.Collapsed;
                BtnClose.Visibility = Visibility.Visible;
            }
            else
            {
                BtnNext.Visibility = Visibility.Visible;
                BtnClose.Visibility = Visibility.Collapsed;
            }

            //Play gifs from start when user flip to them
            var grid = (sender as FlipView).SelectedItem as Grid;
            var img = grid?.Children.OfType<Image>().FirstOrDefault();
            if (img != null && img.Source.GetType() == typeof(BitmapImage))
            {
                var bitmapImg = img.Source as BitmapImage;
                bitmapImg.Stop();//all the gifs start playback when the flipview is loaded
                bitmapImg.Play();//this is "restart"==> user will see them from beginning
            }
        }

        bool lightDismiss;

        //this is here to add light dismiss  functionality
        //copied from https://stackoverflow.com/questions/39317526/uwp-light-dismiss-contentdialog
        protected override void OnApplyTemplate()
        {
            // this is here by default
            base.OnApplyTemplate();

            // get all open popups
            // normally there are 2 popups, one for your ContentDialog and one for Rectangle
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups)
            {
                if (popup.Child is Rectangle)
                {
                    // I store a refrence to Rectangle to be able to unregester event handler later
                    _lockRectangle = popup.Child as Rectangle;
                    _lockRectangle.Tapped += OnLockRectangleTapped;
                }
            }

        }
        Rectangle _lockRectangle;
        private void OnLockRectangleTapped(object sender, TappedRoutedEventArgs e)
        {
            if (lightDismiss)
                this.Hide();
            _lockRectangle.Tapped -= OnLockRectangleTapped;
        }

    }
}
