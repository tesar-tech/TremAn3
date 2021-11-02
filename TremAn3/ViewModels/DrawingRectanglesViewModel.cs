using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.Core.Helpers;

namespace TremAn3.ViewModels
{
    public class DrawingRectanglesViewModel : ViewModelBase
    {
        internal void RemoveRois() => SelectionRectanglesViewModels.Clear();

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
            s.PlotsNeedRefresh += () => plotsNeedRefresh.Invoke();
            return s;
        }

        internal SelectionRectangleViewModel CreateROIFromModel(RoiResultModel roiresultModel)
        {

            SelectionRectangleViewModel s = new SelectionRectangleViewModel(roiresultModel.RoiModel.X, roiresultModel.RoiModel.Y, MaxWidth, MaxHeight, roiresultModel.RoiModel.SizeReductionFactor, roiresultModel.RoiModel.Color);
            s.Width = roiresultModel.RoiModel.Width;
            s.Height = roiresultModel.RoiModel.Height;
            SelectionRectanglesViewModels.Add(s);
            s.DeleteMeAction += selectionToDelete => SelectionRectanglesViewModels.Remove(selectionToDelete);
            s.PlotsNeedRefresh += () => plotsNeedRefresh.Invoke();
            return s;

        }

        public Action plotsNeedRefresh;
        public SelectionRectangleViewModel CurrentRoiInCreationProcess { get; set; }

        private uint _MaxHeight;

        public uint MaxHeight
        {
            get => _MaxHeight;
            set =>
                Set(ref _MaxHeight, value);
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
        /// Adds roi with pos 0,0 and max size. When no roi is added by user (and computation has been started). 
        /// </summary>
        internal void AddMaxRoi()
        {
            var maxRoi =  CreateNewROI(0,0);
            maxRoi.Height = maxRoi.MaxHeight;
            maxRoi.Width = maxRoi.MaxWidth;
        }
    }
}
