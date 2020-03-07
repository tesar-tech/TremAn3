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
    public sealed partial class DrawingRectanglesUc : UserControl
    {

        private DrawingRectanglesViewModel ViewModel
        {
            get { return ViewModelLocator.Current.DrawingRectanglesViewModel; }
        }
        public DrawingRectanglesUc()
        {

            this.InitializeComponent();
            canvas.PointerMoved += canvas_PointerMoved;
            //canvas.PointerEntered += (s, e) => enterWithContact = e.Pointer.IsInContact;
            canvas.PointerReleased += canvas_PointerReleased;
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.CurrentRoiInCreationProcess == null)
                return;
            ViewModel.CurrentRoiInCreationProcess.IsInCreationProcess = false;
                ViewModel.CurrentRoiInCreationProcess = null; 
        }
        //bool loaded;
        //bool enterWithContact;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //loaded = true;
        }

       

        //public double CanvasHeight
        //{
        //    get { return (double)GetValue(CanvasHeightProperty); }
        //    set
        //    {
        //        SetValue(CanvasHeightProperty, value);
        //        if (loaded)//otherwise it throws invalidcast (???) ex
        //        {
        //            SelectionRectangleViewModel.MultiplicatorForUiValues = (int)value / 300;//changes sizes of thickenes and grag cornerc acording to video FramSize
        //            minsize = 50 * (uint)SelectionRectangleViewModel.MultiplicatorForUiValues;
        //        }
        //    }
        //}

        //// Using a DependencyProperty as the backing store for CanvasHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CanvasHeightProperty =
        //    DependencyProperty.Register("CanvasHeight", typeof(double), typeof(DrawingRectangleUc), new PropertyMetadata(0));



        //public double CanvasWidth
        //{
        //    get { return (double)GetValue(CanvasWidthProperty); }
        //    set { SetValue(CanvasWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CanvasWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CanvasWidthProperty =
        //    DependencyProperty.Register("CanvasWidth", typeof(double), typeof(DrawingRectangleUc), new PropertyMetadata(0));



        private Point startPoint;
        private bool manipulationWithRect;
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (manipulationWithRect)
                return;

            startPoint = e.GetCurrentPoint(canvas).Position;
            SelectionRectangleViewModel s =  ViewModel.CreateNewROI(startPoint.X,startPoint.Y);
            ViewModel.CurrentRoiInCreationProcess = s;
            //SelectionRectangleViewModel.IsVisible = true; obnovit
            //SelectionRectangleViewModel.SetValues(startPoint.X, startPoint.Y, 0, 0); obnovit

        }


       

        

        /// <summary>
        /// start of roi drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.CurrentRoiInCreationProcess==null || !e.Pointer.IsInContact ) 
                //contact is neccessary, because release event is not always fired (release outside)
                return;

            var pos = e.GetCurrentPoint(canvas).Position;

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            ViewModel.CurrentRoiInCreationProcess.SetValues(x, y, w, h);
            //SelectionRectangleViewModel.SetValues(x, y, w, h);//obnovit
        }

        private void Viewbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.ChangeSizeProportion(e.NewSize.Width);
        }


    }
}
