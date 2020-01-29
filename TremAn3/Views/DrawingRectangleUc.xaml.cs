using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TremAn3.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TremAn3.Views
{
    public sealed partial class DrawingRectangleUc : UserControl
    {
        public DrawingRectangleUc()
        {
            this.InitializeComponent();
            canvas.PointerMoved += canvas_PointerMoved;
            canvas.PointerEntered += (s, e) => enterWithContact = e.Pointer.IsInContact;
            canvas.PointerReleased += (s, e) => enterWithContact = false;
            GridRoi.PointerPressed += (s, ee) => manipulationWithRect = true;
            GridRoi.ManipulationStarted += (s, ee) => GridRoi.Opacity = 0.5;
            GridRoi.ManipulationCompleted += (s, ee) => { GridRoi.Opacity = 1; manipulationWithRect = false; };
            GridRoi.ManipulationDelta += Roi_ManipulationDelta;
        }

        bool loaded;
        bool enterWithContact;
        double minsize = 50;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

       

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set
            {
                SetValue(CanvasHeightProperty, value);
                if (loaded)//otherwise it throws invalidcast (???) ex
                {
                    SelectionRectangleViewModel.MultiplicatorForUiValues = (int)value / 300;//changes sizes of thickenes and grag cornerc acording to video FramSize
                    minsize = 50 * SelectionRectangleViewModel.MultiplicatorForUiValues;
                }
            }
        }

        // Using a DependencyProperty as the backing store for CanvasHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(DrawingRectangleUc), new PropertyMetadata(0));



        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(DrawingRectangleUc), new PropertyMetadata(0));


        public SelectionRectangleViewModel SelectionRectangleViewModel
        {
            get { return (SelectionRectangleViewModel)GetValue(SelectionRectangleViewModelProperty); }
            set
            {
                SetValue(SelectionRectangleViewModelProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for SelectionRectangleViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionRectangleViewModelProperty =
            DependencyProperty.Register("SelectionRectangleViewModel", typeof(SelectionRectangleViewModel), typeof(DrawingRectangleUc), new PropertyMetadata(0));

        private Point startPoint;
        private bool manipulationWithRect;
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            if (manipulationWithRect)
                return;
            SelectionRectangleViewModel.IsVisible = true;
            startPoint = e.GetCurrentPoint(canvas).Position;
            GridRoi.Width = GridRoi.Height = 0;

            Canvas.SetLeft(GridRoi, startPoint.X);
            Canvas.SetTop(GridRoi, startPoint.Y);
        }

        /// <summary>
        /// dragging corner for roi customization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CornerRectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            FrameworkElement el = (FrameworkElement)sender;
            double x = e.Delta.Translation.X / viewbox.ActualWidth * canvas.ActualWidth;
            double y = e.Delta.Translation.Y / viewbox.ActualHeight * canvas.ActualHeight;

            double top = Canvas.GetTop(GridRoi);
            double left = Canvas.GetLeft(GridRoi);
            double ytop;
            double wleft;

            switch ((string)el.Tag)
            {
                case "lt"://left top
                    wleft = Math.Max(0, left + x);//dont be smaller that top lef cornet (0,0)
                    if (wleft != 0)
                        GridRoi.Width = Math.Max(minsize, GridRoi.Width - x);
                    if (GridRoi.Width != minsize)//move roi with drag
                        Canvas.SetLeft(GridRoi, wleft);

                    ytop = Math.Max(0, top + y);
                    if (ytop != 0)
                        GridRoi.Height = Math.Max(minsize, GridRoi.Height - y);
                    if (GridRoi.Height != minsize)
                        Canvas.SetTop(GridRoi, ytop);

                    break;
                case "rt":
                    wleft = Math.Min(canvas.Width - left, GridRoi.Width + x);
                    GridRoi.Width = Math.Max(minsize, wleft);

                    ytop = Math.Max(0, top + y);
                    if (ytop != 0)
                        GridRoi.Height = Math.Max(minsize, GridRoi.Height - y);
                    if (GridRoi.Height != minsize)
                        Canvas.SetTop(GridRoi, ytop);
                    break;
                case "rb":
                    wleft = Math.Min(canvas.Width - left, GridRoi.Width + x);
                    GridRoi.Width = Math.Max(minsize, wleft);

                    ytop = Math.Min(canvas.Height - top, GridRoi.Height + y);
                    GridRoi.Height = Math.Max(minsize, ytop);
                    break;
                case "lb":
                    wleft = Math.Max(0, left + x);
                    if (wleft != 0)
                        GridRoi.Width = Math.Max(minsize, GridRoi.Width - x);
                    if (GridRoi.Width != minsize)

                        Canvas.SetLeft(GridRoi, wleft);

                    ytop = Math.Min(canvas.Height - top, GridRoi.Height + y);
                    GridRoi.Height = Math.Max(minsize, ytop);
                    break;
                default:
                    break;
            }

            SelectionRectangleViewModel.SetValues(Canvas.GetLeft(GridRoi), Canvas.GetTop(GridRoi), GridRoi.Width, GridRoi.Height);

            e.Handled = true;//donot bubble lower
        }

        /// <summary>
        /// gragging roi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Roi_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var trX = e.Delta.Translation.X / viewbox.ActualWidth * canvas.ActualWidth;
            var trY = e.Delta.Translation.Y / viewbox.ActualHeight * canvas.ActualHeight;

            var setleft = Math.Max(0, Canvas.GetLeft(GridRoi) + trX);//do not cross left upper
            var settop = Math.Max(0, Canvas.GetTop(GridRoi) + trY);

            setleft = Math.Min(setleft, canvas.Width - GridRoi.Width);//do not cross rigth bottom
            settop = Math.Min(settop, canvas.Height - GridRoi.Height);

            Canvas.SetLeft(GridRoi, setleft);
            Canvas.SetTop(GridRoi, settop);
            SelectionRectangleViewModel.SetValues(Canvas.GetLeft(GridRoi), Canvas.GetTop(GridRoi), GridRoi.Width, GridRoi.Height);


        }

        /// <summary>
        /// start of roi drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (manipulationWithRect)
                return;
            if (!e.Pointer.IsInContact || enterWithContact)
                return;

            var pos = e.GetCurrentPoint(canvas).Position;

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            GridRoi.Width = w;
            GridRoi.Height = h;

            Canvas.SetLeft(GridRoi, x);
            Canvas.SetTop(GridRoi, y);
            if (SelectionRectangleViewModel == null)
                SelectionRectangleViewModel = new SelectionRectangleViewModel();
            SelectionRectangleViewModel.SetValues(x, y, w, h);

        }


    }
}
