﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TremAn3.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TremAn3.Views
{
    using OxyPlot.Windows;
    using Windows.System;

    public sealed partial class FreqCounterUc : UserControl
    {

        private TeachingTipsViewModel TeachingTipsViewModel => ViewModelLocator.Current.TeachingTipsViewModel;

        public FreqCounterViewModel ViewModel{ get; set; }

        public SettingsViewModel SettingsViewModel => ViewModelLocator.Current.SettingsViewModel;
        public FreqCounterUc()
        {
            this.InitializeComponent();
        }
    }
}
