using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;

namespace TremAn3.ViewModels
{
    public class SelectionRectangleViewModel : ViewModelBase
    {


        public SelectionRectangleViewModel(double x, double y, double width, double height, uint maxWidth, uint maxHeight)
        {
            X = (uint)Math.Round(x);
            Y = (uint)Math.Round(y);
            Width = (uint)Math.Round(width);
            Height = (uint)Math.Round(height);
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }

        public SelectionRectangleViewModel(double x, double y, uint maxWidth, uint maxHeight)
        {
            X = (uint)Math.Round(x);
            Y = (uint)Math.Round(y);
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }

        private uint _X;

        public uint X
        {
            get => _X;
            set
            {
                //if (value > 1e6) return;
                Set(ref _X, value);
            }
        }
        private uint _Y;

        public uint Y
        {
            get => _Y;
            set
            {
                //if (value > 1e6) return;   
                Set(ref _Y, value);
            }
        }

        private uint _Width;

        public uint Width
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
                Set(ref _Width, value);
            }
        }

        private uint _height;

        public uint Height
        {
            get => _height;
            set
            {
                if (Y + value > MaxHeight)
                {
                    value = MaxHeight - X;
                    if (value == _height)//without this it would not update number in UI when set manualy from max to max+1
                        RaisePropertyChanged();
                }
                Set(ref _height, value);
            }
        }



        //private bool _IsVisible;

        //public bool IsVisible
        //{
        //    get => _IsVisible;
        //    set  {

        //        if (Set(ref _IsVisible, value))
        //            if (!value)//hiding selection will delete the recatngle (and computation will be frome whole frame)
        //                X = Y = Height = Width = 0;

        //    }

        //}

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

        private double _MinSize ;

        public double MinSize
        {
            get => _MinSize;
            set => Set(ref _MinSize, value);
        }


        private void SetUiSizes()
        {
            //keep selection rect to appear same no matter the size of a video
            var ratio = MaxHeight / 300;
            //this method also keeps default values 
            CornerSize = (int)Math.Round(30d * ratio);
            BorderThickness = 2 * ratio;
            MinSize = ratio * 50;
        }




        internal void SetValues(double x, double y, double width, double height)
        {
            X = (uint)Math.Round(x);
            Y = (uint)Math.Round(y);
            Width = (uint)Math.Round(width);
            Height = (uint)Math.Round(height);
        }

        internal SelectionRectangle GetModel(double percentageOfResolution)
        {
            return new SelectionRectangle((X, Y, Width, Height), percentageOfResolution);
        }

        public void DeleteMe()
        {

        }
    }
}
