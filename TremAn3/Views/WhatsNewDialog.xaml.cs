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
            //MarkdownText.Text = text;
        }

        string text = @"
- **Frequency progress**. Now you can see how the main frequency changed during video
  - Segment length setting - How long is segment for single fft computation(in frames) (64,128,256,512,1024)
  - Step setting: How big is step between fft computations(in frames)
- **Help section** in the main menu
- **Tutorial**
  - For new users after the first run.
  - Can be invoked repeatedly from `Help` section.
  - How to videos with text
- **Teaching tips** for better better understanding what to do
  - It runs after first video is loaded
  - Can be run also from `Help` section
-  **Computation in progress label** for better visualization
";

    }
}
