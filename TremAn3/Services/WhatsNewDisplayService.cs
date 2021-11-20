using System;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Helpers;

using TremAn3.Views;

namespace TremAn3.Services
{
    // For instructions on testing this service see https://github.com/Microsoft/WindowsTemplateStudio/tree/master/docs/features/whats-new-prompt.md
    public static class WhatsNewDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {

            if (SystemInformation.Instance.IsAppUpdated && !shown)
            {
                shown = true;
                var dialog = new WhatsNewDialog();
                await dialog.ShowAsync();
            }
        }
    }
}
