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
        if (ViewModel.IsSelectedMeasurementChangeCommingFromSetMethod) return;
        var selectedMeasuremnt = (MeasurementViewModel)e.AddedItems.FirstOrDefault();
        ViewModel.IsSelectedMeasurementChangeCommingFromUi = true;
        await ViewModel.SelectedMeasurementVmSet(selectedMeasuremnt);
        ViewModel.IsSelectedMeasurementChangeCommingFromUi = false;
        //ListView_Measurements.ScrollIntoView(e.AddedItems[0]);
    }
}

public class ListViewWithScrollUp : ListView
{
    //this forces listview to scroll up when new item appears (new measurement is done).
    //https://stackoverflow.com/a/43793412/1154773
    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        if (Items.Count > 0) ScrollIntoView(Items.First());
    }
}
