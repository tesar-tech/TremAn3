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
- Multiple ROIs in one measurement.
- Multiple lines in plots (one for every ROI)
- ROI is in-place editable, has button for closing.
- Export values to CSV. Just visible ones.
- Better column names in exported files (with info about ROI).
- Fixes wrong filename on exported csvs.
- Exports CoMX and CoMY in different files.
- Freq Counter is opened by default (after video is loaded).
- X-Axis in CoM plots are in seconds (not frame number).
  - It also respects the time range.
- Lines on plots are thinner.
";

    }
}
