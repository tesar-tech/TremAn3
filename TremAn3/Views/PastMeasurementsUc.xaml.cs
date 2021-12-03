using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace TremAn3.Views;
public sealed partial class PastMeasurementsUc : UserControl
{

    private PastMeasurementsViewModel ViewModel => ViewModelLocator.Current.PastMeasurementsViewModel;

    public PastMeasurementsUc()
    {
        this.InitializeComponent();
    }

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
            var selectedMeasuremnt = (MeasurementViewModel)e.AddedItems.FirstOrDefault();
            ViewModel.IsSelectedMeasurementChangeCommingFromUi = true;
            await ViewModel.SelectedMeasurementVmSet(selectedMeasuremnt);
            ViewModel.IsSelectedMeasurementChangeCommingFromUi = false;
    }
}
