using System;
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
    using Microsoft.UI.Xaml.Controls;
    using OxyPlot.Series;
    using OxyPlot;
    using OxyPlot.Windows;
    using System.Collections.Specialized;
    using Windows.Globalization.NumberFormatting;
    using Windows.System;
    using TremAn3.Core;

    public sealed partial class FreqCounterUc : UserControl
    {
        private TeachingTipsViewModel TeachingTipsViewModel => ViewModelLocator.Current.TeachingTipsViewModel;

        public FreqCounterViewModel ViewModel { get; set; }

        public SettingsViewModel SettingsViewModel => ViewModelLocator.Current.SettingsViewModel;



        public FreqCounterUc()
        {
            this.InitializeComponent();


        }

        //public  DecimalFormatter DecimalFormatter = new DecimalFormatter() {  FractionDigits = 3};


        private async void NumberBoxMaxHz_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            ViewModel.CurrentGlobalScopedResultsViewModel.CoherenceMaxHz = args.NewValue;
            await ViewModel.CurrentGlobalScopedResultsViewModel.CoherenceRangeChanged();
        }

        private async void NumberBoxMinHz_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            ViewModel.CurrentGlobalScopedResultsViewModel.CoherenceMinHz = args.NewValue;
            await ViewModel.CurrentGlobalScopedResultsViewModel.CoherenceRangeChanged();
        }

        private void ScrollToEndOfCoherenceResults()
        {
            if (ListViewCoherenceResults.Items.Count > 0)
                ListViewCoherenceResults.ScrollIntoView(ListViewCoherenceResults.Items[ListViewCoherenceResults.Items.Count - 1]);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            ViewModelLocator.Current.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel.AddedToCollection = ScrollToEndOfCoherenceResults;
        }



        private async void PlotView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var plotView = sender as PlotView;
            DataSeriesType? dataSeriesType = (DataSeriesType?)plotView.Tag;
            var collector = ViewModel.CurrentGlobalScopedResultsViewModel.GetPointsCollectorVm((DataSeriesType)dataSeriesType);
            if (!collector.IsInCollectingState) return;
            

            if (plotView?.Model != null)
            {
                var position = e.GetCurrentPoint(plotView).Position;
                double? xCoord= GetXCoordinate(plotView, position);
                if (xCoord != null)
                {
                    await collector.AddPoint(xCoord.Value);
                }
            }
        }
        private double? GetXCoordinate(PlotView plotView, Windows.Foundation.Point position)
        {
            var actualModel = plotView.ActualModel;
            if (actualModel != null)
            {
                var inverseTransform = plotView.ActualModel.DefaultXAxis.InverseTransform(position.X);
                return inverseTransform;
            }
            return null;
        }

    }
}
