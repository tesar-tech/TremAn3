using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TremAn3.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            // TODO WTS: Update the contents of this dialog every time you release a new version of the app
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
            MarkdownText.Text = text;
        }

        string text = @"
- ROI implementation - works better on small details and insignificant movement.
  - ROI is customizable (size and position changes)
  - Also works with resolution reduction(speed boost).
  - Works with window size changes.
- Store badge in README  
- In app notifications
- CSV export
- Support for x64(fixes ffmpeginterop package)
- FFmpegInteropX in nuget package";

    }
}
