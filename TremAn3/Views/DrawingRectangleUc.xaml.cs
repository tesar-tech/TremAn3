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
        }

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
        private Point endPoint;
        private Rectangle rect;

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            canvas.Children.Clear();
            startPoint = e.GetCurrentPoint(canvas).Position;
            rect = new Rectangle()
            {
                Stroke = new SolidColorBrush(Color.FromArgb(255,112,255,0)),
                StrokeThickness = 5
            };

            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            canvas.Children.Add(rect);
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact || rect == null)
                return;

            var pos = e.GetCurrentPoint(canvas).Position;

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            endPoint = pos;
            Rect = ((uint)x,(uint)y,(uint)w,(uint)h);
           
        }

        private  void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            rect = null;
        }

    }
}
