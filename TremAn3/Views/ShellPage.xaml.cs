using System;
using TremAn3.Helpers;
using TremAn3.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TremAn3.Views
{
    // TODO WTS: You can edit the text for the menu in String/en-US/Resources.resw
    // You can show pages in different ways (update main view, navigate, right pane, new windows or dialog) using MenuNavigationHelper class.
    // Read more about MenuBar project type here:
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/projectTypes/menubar.md
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            ViewModel.Initialize(shellFrame, splitView, rightFrame, KeyboardAccelerators);
        }

      
        private async void Button_ClickAsync(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await MenuNavigationHelper.OpenInNewWindow(typeof(HelpPage));
        }
    }
}
