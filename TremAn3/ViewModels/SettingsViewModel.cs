using System;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TremAn3.Helpers;
using TremAn3.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace TremAn3.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : ViewModelBase
    {

    

        private string _DecimalSeparator = LocalSettings.Read(".", nameof(DecimalSeparator));

        public string DecimalSeparator
        {
            get => _DecimalSeparator;
            set
            {
                if (_DecimalSeparator == value) return;
                if (value != "")
                {
                    _DecimalSeparator = value;
                    LocalSettings.Write(value);
                }
               
                RaisePropertyChanged();
            }
        }

        private string _CsvElementSeparator = LocalSettings.Read(",", nameof(CsvElementSeparator));

        public string CsvElementSeparator
        {
            get => _CsvElementSeparator;
            set
            {
                if (_CsvElementSeparator == value) return;
                if (!string.IsNullOrEmpty(value))
                {
                    //_CsvElementSeparator = value.Replace(@"\t","\t");
                    _CsvElementSeparator = value;
                    LocalSettings.Write(value);
                }
                RaisePropertyChanged();
            }
        }




        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        private bool _IsLoadRecentVideoOnAppStart = LocalSettings.Read(true, nameof(IsLoadRecentVideoOnAppStart));

        public bool IsLoadRecentVideoOnAppStart
        {
            get => _IsLoadRecentVideoOnAppStart;
            set
            {
                if (!Set(ref _IsLoadRecentVideoOnAppStart, value)) return;
                LocalSettings.Write(value);
            }
        }

        public SettingsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            return $"{appName} - {Helpers.Helpers.VersionOfApp}";
        }
    }
}
