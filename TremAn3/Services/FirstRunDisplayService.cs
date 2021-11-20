using System;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Helpers;

using TremAn3.Views;

namespace TremAn3.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {
            if (SystemInformation.Instance.IsFirstRun && !shown)
            {
                shown = true;
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }
        }
    }
}
