using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            canvas.PointerEntered += (s,e) => enterWithContact = e.Pointer.IsInContact;
            canvas.PointerReleased += (s, e) => enterWithContact = false;
        }

        bool enterWithContact;
        public void ClearCanvas()
        {
            canvas.Children.Clear();
        }

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
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




        public (uint X, uint Y, uint width, uint height) Rect
        {
            get { return ((uint X, uint Y, uint width, uint height))GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof((uint X, uint Y, uint width, uint height)), typeof(DrawingRectangleUc), new PropertyMetadata(0));


        private Point startPoint;
        private bool manipulationWithRect;
        Grid gr;

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (manipulationWithRect)
                return;
           
            canvas.Children.Clear();
            startPoint = e.GetCurrentPoint(canvas).Position;
            CreateRoiUi();
           
            Canvas.SetLeft(gr, startPoint.X);
            Canvas.SetTop(gr, startPoint.Y);
            canvas.Children.Add(gr);
        }

      
        private void CreateRoiUi()
        {
            //border (keeps style)
           var bor = new Border()
            {
                Background = new SolidColorBrush(Color.FromArgb(30, 112, 255, 0)),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 112)),
            };
            gr = new Grid();

            gr.Children.Add(bor);
            //grid manipulation
            gr.PointerPressed += (s, ee) => manipulationWithRect = true;
            gr.ManipulationStarted += (s, ee) => gr.Opacity = 0.5;
            gr.ManipulationCompleted += (s, ee) => { gr.Opacity = 1; manipulationWithRect = false; };
            gr.ManipulationDelta += Roi_ManipulationDelta;
            gr.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;


            //corners for drag
            var alignments = new List<(HorizontalAlignment, VerticalAlignment, string)>
            {
                (HorizontalAlignment.Left,VerticalAlignment.Top,"lt"),
                (HorizontalAlignment.Right,VerticalAlignment.Top,"rt"),
                (HorizontalAlignment.Right,VerticalAlignment.Bottom,"rb"),
                (HorizontalAlignment.Left,VerticalAlignment.Bottom,"lb")
            };

            foreach (var a in alignments)
            {
                Rectangle re = new Rectangle();
                gr.Children.Add(re);
                re.Fill = new SolidColorBrush(Color.FromArgb(50, 143, 0, 255));
                re.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                re.Height = 30;
                re.Width = 30;
                re.HorizontalAlignment = a.Item1;
                re.VerticalAlignment = a.Item2;
                re.ManipulationDelta += CornerRectangle_ManipulationDelta;
                re.Tag = a.Item3;//as an ID of button, in manipulation delta
            }

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


            double top = Canvas.GetTop(gr);
            double left = Canvas.GetLeft(gr);
            double ytop;
            double wleft;
            double minsize = 50;

            switch ((string)el.Tag)
            {
                case "lt"://left top
                     wleft = Math.Max(0, left + x);//dont be smaller that top lef cornet (0,0)
                    if (wleft != 0)
                        gr.Width = Math.Max(minsize, gr.Width - x);
                    if (gr.Width != minsize)//move roi with drag
                        Canvas.SetLeft(gr, wleft);

                     ytop = Math.Max(0, top + y);
                    if (ytop != 0)
                        gr.Height = Math.Max(minsize, gr.Height - y);
                    if (gr.Height != minsize)
                        Canvas.SetTop(gr, ytop);

                    break;
                case "rt":
                     wleft = Math.Min(canvas.Width - left, gr.Width + x);
                    gr.Width = Math.Max(minsize, wleft);

                    ytop = Math.Max(0, top + y);
                    if (ytop !=0)
                        gr.Height = Math.Max(minsize, gr.Height - y);
                    if (gr.Height != minsize)
                        Canvas.SetTop(gr, ytop);
                    break;
                case "rb":
                    wleft = Math.Min(canvas.Width - left, gr.Width + x);
                    gr.Width = Math.Max(minsize, wleft);

                    ytop = Math.Min(canvas.Height - top, gr.Height + y);
                    gr.Height = Math.Max(minsize, ytop);
                    break;
                case "lb":
                    wleft = Math.Max(0, left + x);
                    if (wleft != 0)
                        gr.Width = Math.Max(minsize, gr.Width - x);
                    if (gr.Width != minsize)

                        Canvas.SetLeft(gr, wleft);

                    ytop = Math.Min(canvas.Height - top, gr.Height + y);
                    gr.Height = Math.Max(minsize, ytop);
                    break;
                default:
                    break;
            }

            Rect = ((uint)Canvas.GetLeft(gr), (uint)Canvas.GetTop(gr), (uint)gr.Width, (uint)gr.Height);

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

            var setleft = Math.Max(0, Canvas.GetLeft(gr) + trX);//do not cross left upper
            var settop = Math.Max(0, Canvas.GetTop(gr) + trY);

            setleft = Math.Min(setleft, canvas.Width - gr.Width);//do not cross rigth bottom
            settop = Math.Min(settop, canvas.Height - gr.Height);

            Canvas.SetLeft(gr, setleft);
            Canvas.SetTop(gr, settop);
            Rect = ((uint)Canvas.GetLeft(gr), (uint)Canvas.GetTop(gr), (uint)gr.Width, (uint)gr.Height);

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
            if (!e.Pointer.IsInContact || gr == null || enterWithContact)
                return;

            var pos = e.GetCurrentPoint(canvas).Position;

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            gr.Width = w;
            gr.Height = h;

            Canvas.SetLeft(gr, x);
            Canvas.SetTop(gr, y);
            Rect = ((uint)x, (uint)y, (uint)w, (uint)h);

        }
    }
}
