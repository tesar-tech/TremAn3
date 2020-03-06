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
            SelectionRectangleViewModel s1 = new SelectionRectangleViewModel();
            s1.MaxHeight = 376;s1.MaxWidth = 500;s1.IsVisible = true;
            s1.SetValues(50, 60, 100, 200); SelectionRectanglesViewModels.Add(s1);

            SelectionRectangleViewModel s2 = new SelectionRectangleViewModel();
            s2.MaxHeight = 376; s2.MaxWidth = 500; s2.IsVisible = true;
            s2.SetValues(120, 80, 80, 60); SelectionRectanglesViewModels.Add(s2);

           
        }

        private ObservableCollection<SelectionRectangleViewModel> _SelectionRectanglesViewModels = new ObservableCollection<SelectionRectangleViewModel>();

        public ObservableCollection<SelectionRectangleViewModel> SelectionRectanglesViewModels
        {
            get => _SelectionRectanglesViewModels;
            set => Set(ref _SelectionRectanglesViewModels, value);
        }

    }
}
