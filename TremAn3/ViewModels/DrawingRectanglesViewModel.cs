using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
    public class DrawingRectanglesViewModel : ViewModelBase
    {
        public DrawingRectanglesViewModel()
        {
            //DEBUG data
           // SelectionRectangleViewModel s1 = new SelectionRectangleViewModel(50, 60, 100, 200,500,376);
           //SelectionRectanglesViewModels.Add(s1);

           // SelectionRectangleViewModel s2 = new SelectionRectangleViewModel(120, 80, 80, 60,500,376);
           //SelectionRectanglesViewModels.Add(s2);

           
        }

        private ObservableCollection<SelectionRectangleViewModel> _SelectionRectanglesViewModels = new ObservableCollection<SelectionRectangleViewModel>();

        internal void RemoveRois()
        {
            SelectionRectanglesViewModels.Clear();
        }

        public ObservableCollection<SelectionRectangleViewModel> SelectionRectanglesViewModels
        {
            get => _SelectionRectanglesViewModels;
            set => Set(ref _SelectionRectanglesViewModels, value);
        }

        internal SelectionRectangleViewModel CreateNewROI(double x, double y)
        {
            SelectionRectangleViewModel s = new SelectionRectangleViewModel(x,y,MaxWidth,MaxHeight);
            SelectionRectanglesViewModels.Add(s);
            return s;
        }

        public SelectionRectangleViewModel CurrentRoiInManipulation { get; set; }

        private uint _MaxHeight;

        public uint MaxHeight
        {
            get => _MaxHeight;
            set
            {
                Set(ref _MaxHeight, value);
                //SetUiSizes();
            }
        }

        private uint _MaxWidth;
        public uint MaxWidth
        {
            get => _MaxWidth;
            set => Set(ref _MaxWidth, value);
        }
    }
}
