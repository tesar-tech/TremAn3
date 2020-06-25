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
- Computation could be canceled.
  - The button isn't disabled, the text is changed to `Cancel`.
  - It works with the `Esc` key.
- Now you can open video by drag&drop.
  - When an unsupported file type is dropped, a notification with info will pop up.
  - When dropping multiple files, the first supported will be opened.
- Info about video file in title of the app window.
  - Name, resolution, frame rate, size in MB.
";

    }
}
