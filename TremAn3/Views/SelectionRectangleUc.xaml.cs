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
        }
        bool manipulationWithRect;
        /// <summary>
        /// gragging roi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Roi_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var trX = e.Delta.Translation.X /*/ viewbox.ActualWidth * canvas.ActualWidth*/;//obnovit
            
            var trY = e.Delta.Translation.Y /*/ viewbox.ActualHeight * canvas.ActualHeight*/;//obnovit

            var setleft = Math.Max(0,  ViewModel.X + trX);//do not cross left upper
            var settop = Math.Max(0,  ViewModel.Y + trY);

            setleft = Math.Min(setleft, ViewModel.MaxWidth-  ViewModel.Width);//do not cross rigth bottom
            settop = Math.Min(settop, ViewModel.MaxHeight -  ViewModel.Height);

             ViewModel.X = (uint)setleft;
             ViewModel.Y = (uint)settop;
        }

        uint minsize = 50;


        /// <summary>
        /// dragging corner for roi customization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CornerRectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            FrameworkElement el = (FrameworkElement)sender;
            var trX = e.Delta.Translation.X /*/ viewbox.ActualWidth * canvas.ActualWidth*/;//obnovit
            var trY = e.Delta.Translation.Y /*/ viewbox.ActualHeight * canvas.ActualHeight*/;//obnovit

            //double trX = (e.Delta.Translation.X / viewbox.ActualWidth * canvas.ActualWidth);
            //double trY = (e.Delta.Translation.Y / viewbox.ActualHeight * canvas.ActualHeight);

            uint top =  ViewModel.Y;
            uint left =  ViewModel.X;
            uint ytop;
            uint wleft;

            switch ((string)el.Tag)
            {
                case "lt"://left top
                    wleft = (uint)Math.Round(Math.Max(0, left + trX));//dont be smaller that top lef cornet (0,0)
                    if (wleft != 0)
                         ViewModel.Width = (uint)Math.Round(Math.Max(minsize,  ViewModel.Width - trX));
                    if ( ViewModel.Width != minsize)//move roi with drag
                         ViewModel.X = wleft;

                    ytop = (uint)Math.Round(Math.Max(0, top + trY));
                    if (ytop != 0)
                         ViewModel.Height = (uint)Math.Round(Math.Max(minsize,  ViewModel.Height - trY));
                    if ( ViewModel.Height != minsize)
                         ViewModel.Y = ytop;
                    break;
                case "rt":
                    wleft = (uint)Math.Round(Math.Min(ViewModel.MaxWidth- left,  ViewModel.Width + trX));
                     ViewModel.Width = Math.Max(minsize, wleft);

                    ytop = (uint)Math.Round(Math.Max(0, top + trY));
                    if (ytop != 0)
                         ViewModel.Height = (uint)Math.Round(Math.Max(minsize,  ViewModel.Height - trY));
                    if ( ViewModel.Height != minsize)
                         ViewModel.Y = ytop;

                    break;
                case "rb":
                    wleft = (uint)Math.Round(Math.Min(ViewModel.MaxWidth - left,  ViewModel.Width + trX));
                     ViewModel.Width = Math.Max(minsize, wleft);

                    ytop = (uint)Math.Min(ViewModel.MaxHeight - top,  ViewModel.Height + trY);
                     ViewModel.Height = Math.Max(minsize, ytop);
                    break;
                case "lb":
                    wleft = (uint)Math.Round(Math.Max(0, left + trX));
                    if (wleft != 0)
                         ViewModel.Width = (uint)Math.Round(Math.Max(minsize,  ViewModel.Width - trX));
                    if ( ViewModel.Width != minsize)

                         ViewModel.X = wleft;

                    ytop = (uint)Math.Round(Math.Min(ViewModel.MaxHeight - top,  ViewModel.Height + trY));
                     ViewModel.Height = Math.Max(minsize, ytop);
                    break;
                default:
                    break;
            }


            e.Handled = true;//donot bubble lower
        }
    }
}
