using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TremAn3.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;
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
    public sealed partial class SelectionRectangleUc : UserControl
    {


        public SelectionRectangleViewModel ViewModel { get; set; }
        //public SelectionRectangleViewModel ViewModel
        //{
        //    get { return (SelectionRectangleViewModel)GetValue(ViewModelProperty); }
        //    set
        //    {
        //        SetValue(ViewModelProperty, value);
        //    }
        //}

        //// Using a DependencyProperty as the backing store for SelectionRectangleViewModel.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ViewModelProperty =
        //    DependencyProperty.Register("ViewModel", typeof(SelectionRectangleViewModel), typeof(SelectionRectangleUc), new PropertyMetadata(0));

        public SelectionRectangleUc()
        {
            this.InitializeComponent();
            GridRoi.PointerPressed += (s, ee) => { manipulationWithRect = true;ee.Handled = true; };
            GridRoi.ManipulationStarted += (s, ee) => GridRoi.Opacity = 0.5;
            GridRoi.ManipulationCompleted += (s, ee) => { GridRoi.Opacity = 1; manipulationWithRect = false; };
            GridRoi.ManipulationDelta += Roi_ManipulationDelta;
            SetFormatersForNumberBoxes();

        }

        private void SetFormatersForNumberBoxes()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 1;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;
            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 0;
            formatter.NumberRounder = rounder;
            nb1.NumberFormatter = formatter;
            nb2.NumberFormatter = formatter;
            nb3.NumberFormatter = formatter;
            nb4.NumberFormatter = formatter;
        }


        bool manipulationWithRect;
        /// <summary>
        /// gragging roi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Roi_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var trX = e.Delta.Translation.X * ViewModel.SizeProportion;
            var trY = e.Delta.Translation.Y * ViewModel.SizeProportion;

            var setleft = Math.Max(0,  ViewModel.X + trX);//do not cross left upper
            var settop = Math.Max(0,  ViewModel.Y + trY);

            setleft = Math.Min(setleft, ViewModel.MaxWidth-  ViewModel.Width);//do not cross rigth bottom
            settop = Math.Min(settop, ViewModel.MaxHeight -  ViewModel.Height);

             ViewModel.X = setleft;
             ViewModel.Y = settop;
        }

        //uint minsize = 50;


        /// <summary>
        /// dragging corner for roi customization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CornerRectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            FrameworkElement el = (FrameworkElement)sender;
            var trX = e.Delta.Translation.X * ViewModel.SizeProportion;
            var trY = e.Delta.Translation.Y * ViewModel.SizeProportion;

            double top =  ViewModel.Y;
            double left = ViewModel.X;
            double ytop;
            double wleft;

            switch ((string)el.Tag)
            {
                case "lt"://left top
                    wleft = Math.Max(0, left + trX);//dont be smaller that top lef cornet (0,0)
                    if (wleft != 0)
                         ViewModel.Width = Math.Max(ViewModel.MinSize,  ViewModel.Width - trX);
                    if ( ViewModel.Width != ViewModel.MinSize)//move roi with drag
                         ViewModel.X = wleft;

                    ytop = Math.Max(0, top + trY);
                    if (ytop != 0)
                         ViewModel.Height = Math.Max(ViewModel.MinSize,  ViewModel.Height - trY);
                    if ( ViewModel.Height != ViewModel.MinSize)
                         ViewModel.Y = ytop;
                    break;
                case "rt":
                    wleft = Math.Min(ViewModel.MaxWidth- left,  ViewModel.Width + trX);
                     ViewModel.Width = Math.Max(ViewModel.MinSize, wleft);

                    ytop = Math.Max(0, top + trY);
                    if (ytop != 0)
                         ViewModel.Height = Math.Max(ViewModel.MinSize,  ViewModel.Height - trY);
                    if ( ViewModel.Height != ViewModel.MinSize)
                         ViewModel.Y = ytop;

                    break;
                case "rb":
                    wleft = Math.Min(ViewModel.MaxWidth - left,  ViewModel.Width + trX);
                     ViewModel.Width = Math.Max(ViewModel.MinSize, wleft);

                    // max height for current location vs what i want with drag
                    ytop = Math.Min(ViewModel.MaxHeight - top,  ViewModel.Height + trY);
                     ViewModel.Height = Math.Max(ViewModel.MinSize, ytop);
                    break;
                case "lb":
                    wleft = Math.Max(0, left + trX);
                    if (wleft != 0)
                         ViewModel.Width = Math.Max(ViewModel.MinSize,  ViewModel.Width - trX);
                    if ( ViewModel.Width != ViewModel.MinSize)

                         ViewModel.X = wleft;

                    ytop = Math.Min(ViewModel.MaxHeight - top,  ViewModel.Height + trY);
                     ViewModel.Height = Math.Max(ViewModel.MinSize, ytop);
                    break;
                default:
                    break;
            }


            e.Handled = true;//donot bubble lower
        }
    }
}
