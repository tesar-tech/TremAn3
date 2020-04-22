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
- Time range selector is within playback slider.
  - More intuitive placement.
- Secondary slider is added to proximity with CoM plots.
  - Helps to understand relation between CoM plots and video.
  - Only shows the selected time range (same as plots).
- Vertical line in CoM plots to view correlation between video and CoM movement.
  - Line follows video time position.
  - Current video position is visibile in plots.
- Mild flash on `Count freq` button as a warning of obsolete results.
  - When plots are displayed, and ROI is moved it isn't in correlation anymore. So plot disappears and button starts flashing.
- Buttons for making plots bigger.
- Removes unnecessary buttons from playback area.
- `No data` label for plots without data.
";

    }
}
