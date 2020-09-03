using System;
using TremAn3.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TremAn3.Views
{
    public sealed partial class DrawingRectanglesUc : UserControl
    {

        private TeachingTipsViewModel TeachingTipsViewModel => ViewModelLocator.Current.TeachingTipsViewModel;

        private DrawingRectanglesViewModel ViewModel => ViewModelLocator.Current.DrawingRectanglesViewModel;
        public DrawingRectanglesUc()
        {
            this.InitializeComponent();
            canvas.PointerMoved += canvas_PointerMoved;
            canvas.PointerReleased += canvas_PointerReleased;
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.CurrentRoiInCreationProcess == null)
                return;
            ViewModel.CurrentRoiInCreationProcess.IsInCreationProcess = false;
                ViewModel.CurrentRoiInCreationProcess = null; 
        }

        private Point startPoint;
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            startPoint = e.GetCurrentPoint(canvas).Position;
            SelectionRectangleViewModel s =  ViewModel.CreateNewROI(startPoint.X,startPoint.Y);
            ViewModel.CurrentRoiInCreationProcess = s;
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
        }

        private void Viewbox_SizeChanged(object sender, SizeChangedEventArgs e) => ViewModel.ChangeSizeProportion(e.NewSize.Width);

     
    }
}
