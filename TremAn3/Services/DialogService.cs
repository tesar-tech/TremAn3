using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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


    }
}
