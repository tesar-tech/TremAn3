using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Views;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TremAn3.Services
{
    internal class DialogService
    {
        public static async Task<bool> DeleteMeasurementDialog(string title = "Delete that measurement?")
        {

            ContentDialog dialog = new ContentDialog();
            dialog.Title = title;
            dialog.PrimaryButtonText = "Yes, delete it!";
            dialog.CloseButtonText = "No, get me back!";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "You sure about that?";

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public static async Task<string> EditNameDialog(string oldName)
        {
            
            var inputTextBox = new TextBox { AcceptsReturn = false};
            if(oldName != null) inputTextBox.Text = oldName;
            inputTextBox.VerticalAlignment = VerticalAlignment.Bottom;
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = "Edit display name",
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return null;
        }

        internal static async Task DisplaySpectralAnalysisInfo()
        {
            MarkdownTextBlock txbl = new MarkdownTextBlock()
            {
                
                Text = @"

# PSD -  Power Spectral density
Matches function periodogram in matlab.

```matlab
periodogram(x,rectwin(length(x)),length(x),Fs)
```

**X axis**: Frequency (Hz). From 0 to frame rate /2

**Y axis**: Power/Frequency (db/Hz)

# AmpSpec -  Amplitude spectrum

Amplitudue spectrum from whole signal, no window, no averaging.

```matlab
amp_spec = abs(fft(x));
amp_spec = amp_spec(1:length(x)/2+1);
```

**X axis**: Frequency (Hz). From 0 to frame rate /2

**Y axis**: pixels/Hz

# Welch -  Welch’s power spectral density estimate

With window of length of 256 and overlap of 255 (jups of size 1). pwelch function in matlab.

```matlab
window= hamming(256);
pw = pwelch(x,window,255);
```

**X axis**: Frequency (Hz). From 0 to frame rate /2

**Y axis**: squared pixels/Hz

# Coherence -  Magnitude-squared coherence

The magnitude-squared coherence estimate is a function of frequency with values between 0 and 1. These values indicate how well x corresponds to y at each frequency. The magnitude-squared coherence is a function of the power spectral densities, Pxx(f) and Pyy(f), and the cross power spectral density, Pxy(f), of x and y:

C<sub>xy</sub>(f)=|P<sub>xy</sub>(f)|<sup>2</sup>  / (P<sub>xx</sub>(f) P<sub>yy</sub>(f))

(more information inside mscohere function in matlab)

Treman implamantation uses window of length 256 and overlap of 255 (jups of size 1), fft size = 256. 

```matlab
window= hamming(256); fs = frameRate;
cohe = mscohere(x,y,window,255,256,fs);
```

**X axis**: Frequency (Hz). From 0 to frame rate /2

**Y axis**: () from 0 to 1

",
                Padding = new Thickness(10.0),
                Header2Margin = new Thickness(1),
                Header1Margin = new Thickness(3),
                ParagraphMargin = new Thickness(0),
                CodeMargin = new Thickness(0),
                CodeForeground = Application.Current.Resources["SystemControlBackgroundBaseMediumBrush"] as SolidColorBrush,
                Header1Foreground = Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush,
            };
            
            ScrollViewer sv = new ScrollViewer()
            {
                Content = txbl
            };
            var dialog = new ContentDialog
            {
                Content = sv,
                Title = "Info",
                PrimaryButtonText = "Ok",
            };
          await  dialog.ShowAsync();
            //SpectralAnalysisInfoDialog sd = new SpectralAnalysisInfoDialog();
            //await sd.ShowAsync();
        }
    }
}
