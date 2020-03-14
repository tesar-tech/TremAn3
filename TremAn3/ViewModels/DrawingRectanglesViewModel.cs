using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core.Helpers;

namespace TremAn3.ViewModels
{
    public class DrawingRectanglesViewModel : ViewModelBase
    {
        public DrawingRectanglesViewModel()
        {
            //DEBUG data
            // SelectionRectangleViewModel s1 = new SelectionRectangleViewModel(50, 60, 100, 200,500,376);
            //SelectionRectanglesViewModels.Add(s1);

            //SelectionRectangleViewModel s2 = new SelectionRectangleViewModel(120, 80, 80, 60, 500, 376);
            //SelectionRectanglesViewModels.Add(s2);


        }


        internal void RemoveRois()
        {
            SelectionRectanglesViewModels.Clear();
        }

        private ObservableCollection<SelectionRectangleViewModel> _SelectionRectanglesViewModels = new ObservableCollection<SelectionRectangleViewModel>();

        public ObservableCollection<SelectionRectangleViewModel> SelectionRectanglesViewModels
        {
            get => _SelectionRectanglesViewModels;
            set => Set(ref _SelectionRectanglesViewModels, value);
        }

        internal SelectionRectangleViewModel CreateNewROI(double x, double y)
        {
            var color = ColorHelper.GetNextColorThatIsNotInList( SelectionRectanglesViewModels.Select(c => c.Color).ToList());
            SelectionRectangleViewModel s = new SelectionRectangleViewModel(x,y,MaxWidth,MaxHeight,currentSizeProportion,color);
            SelectionRectanglesViewModels.Add(s);
            s.DeleteMeAction += selectionToDelete => SelectionRectanglesViewModels.Remove(selectionToDelete);
            s.plotsNeedRefresh += () => plotsNeedRefresh.Invoke();
            return s;
        }

        public Action plotsNeedRefresh;
        public SelectionRectangleViewModel CurrentRoiInCreationProcess { get; set; }

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


        private double currentSizeProportion;
        public void ChangeSizeProportion(double viewBoxWidth)
        {
            currentSizeProportion = MaxWidth / viewBoxWidth;
            foreach (var roi in SelectionRectanglesViewModels)
                roi.SizeProportion = currentSizeProportion;
        }

        /// <summary>
        /// Adds roi with pos 0,0 and max size. When no roi is added by user. 
        /// </summary>
        internal void AddMaxRoi()
        {
            var maxRoi =  CreateNewROI(0,0);
            maxRoi.Height = maxRoi.MaxHeight;
            maxRoi.Width = maxRoi.MaxWidth;
        }
    }
}
