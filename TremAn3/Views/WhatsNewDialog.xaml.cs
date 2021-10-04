using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TremAn3.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        //this shows link to release page on gh
        public WhatsNewDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }
    }
}
