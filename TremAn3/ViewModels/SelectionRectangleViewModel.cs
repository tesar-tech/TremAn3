using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;

namespace TremAn3.ViewModels
{
    public class SelectionRectangleViewModel : ViewModelBase
    {

        public SelectionRectangleViewModel(double x, double y, uint maxWidth, uint maxHeight,double sizeProportion, Color color)
        {
            X = (uint)Math.Round(x);
            Y = (uint)Math.Round(y);

            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            IsInCreationProcess = true;
            SizeProportion = sizeProportion;
            Color = color;
            ComputationViewModel = new SelectionRectangleComputationViewModel(Color,this);

        }


        internal void InitializeCoM(int decodedPixelWidth,int decodedPixelHeight,double frameRate, double percentageOfResolution)
        {
           var rect = GetModel(percentageOfResolution);
            ComputationViewModel.InitializeCoM(decodedPixelWidth, decodedPixelHeight, frameRate, rect);
        }

        public SelectionRectangleComputationViewModel ComputationViewModel { get; set; }

        private Color _Color;

        public Color Color
        {
            get => _Color;
            set => Set(ref _Color, value);
        }



        private double _X;

        public double X
        {
            get => _X;
            set
            {
                //if (value > 1e6) return;
                if(Set(ref _X, value))
                RoiChanged();

            }
        }
        private double _Y;

        public double Y
        {
            get => _Y;
            set
            {
                //if (value > 1e6) return;   
               if( Set(ref _Y, value))
                RoiChanged();
            }
        }

        private void RoiChanged()
        {
            if(!(ComputationViewModel is null))
            ComputationViewModel.IsRoiSameAsResult = false;
        }

        private double _Width;

        public double Width
        {
            get => _Width;
            set

            {
                if (X + value > MaxWidth)
                {
                    value = MaxWidth - X;
                    if (value == _Width)//without this it would not update number in UI when set manualy from max to max+1
                        RaisePropertyChanged();
                }
                if (value < MinSize && !IsInCreationProcess)//smaller than minsize
                {
                    value = MinSize;
                    RaisePropertyChanged();//otherwise it willnot update the ui (prop is not changed here, but on ui is)
                }

                if(Set(ref _Width, value))
                    RoiChanged();

            }
        }

        private double _height;

        public double Height
        {
            get => _height;
            set
            {
                if (Y + value > MaxHeight)//bigger than max
                {
                    value = MaxHeight - Y;//this little forgotten X took me 30 minutes..
                    if (value == _height)//without this it would not update number in UI when set manualy from max to max+1
                        RaisePropertyChanged();
                }
                if (value < MinSize && !IsInCreationProcess)//smaller than minsize
                {
                    value = MinSize;
                    RaisePropertyChanged();//otherwise it willnot update the ui (prop is not changed here, but on ui is)
                }
                if(Set(ref _height, value))
                    RoiChanged();
            }
        }



        public Action plotsNeedRefresh { get; set; }

        private bool _IsShowInPlot = true;

        public bool IsShowInPlot
        {
            get => _IsShowInPlot;
            set {
                bool wasChange = Set(ref _IsShowInPlot, value);
                if (wasChange)
                {
                    ComputationViewModel.ChangeVisibilityOfLines(_IsShowInPlot);
                    plotsNeedRefresh.Invoke();                   
                }
            }
        }


        private double _BorderThickness = 2;

        public double BorderThickness
        {
            get => _BorderThickness;
            set => Set(ref _BorderThickness, value);
        }

        private int _CornerSize = 30;

        public int CornerSize
        {
            get => _CornerSize;
            set => Set(ref _CornerSize, value);
        }




        private uint _MaxHeight;

        public uint MaxHeight
        {
            get => _MaxHeight;
            set
            {
                _MaxHeight = value;
                //Set(ref _MaxHeight, value);
                SetUiSizes();
            }
        }



        public uint MaxWidth;

        //public uint MaxWidth
        //{
        //    get => _MaxWidth;
        //    set => Set(ref _MaxWidth, value);
        //}

        private double _MinSize;

        public double MinSize
        {
            get => _MinSize;
            set => Set(ref _MinSize, value);
        }

        private bool isInCreationProcess;

        public bool IsInCreationProcess { get => isInCreationProcess;
            set {
                if (isInCreationProcess && !value)//from creation to completion
                { //will fixes size after roi is created
                 
                    isInCreationProcess = false;
                    Width = Width;
                    Height = Height;

                }else
               isInCreationProcess = value;
            }
        }

        public double SizeProportion { get; internal set; }

        private void SetUiSizes()
        {
            //keep selection rect to appear same no matter the size of a video
            double ratio = MaxHeight / 300d;
            //this method also keeps default values 
            CornerSize = (int)Math.Round(30d * ratio);
            BorderThickness = 1.2 * ratio;
            MinSize = ratio * 50;
        }




        internal void SetValues(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal SelectionRectangle GetModel(double percentageOfResolution)
        {
            return new SelectionRectangle((X, Y, Width, Height), percentageOfResolution);
        }

        public override string ToString()
        {
            return $"roi_X{Math.Round(X)}_Y{Math.Round(Y)}_W{Math.Round(Width)}_H{Math.Round(Height)}";
        }

        public event Action<SelectionRectangleViewModel> DeleteMeAction;

        public void DeleteMe()
        {
            if (ComputationViewModel != null)
                ComputationViewModel.IsRoiSameAsResult = false;
            DeleteMeAction.Invoke(this);//it is subscribed in drawing rectangles
        }
    }
}
