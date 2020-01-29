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
            set {
                //if (value > 1e6) return;   
                Set(ref _Y, value);
            }
        }

        private uint _Width;

        public uint Width
        {
            get => _Width;
            set => Set(ref _Width, value);
        }

        private uint _height;

        public uint Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        

        private bool _IsVisible;

        public bool IsVisible
        {
            get => _IsVisible;
            set => Set(ref _IsVisible, value);

        }

        private int _BorderThickness = 2;

        public int BorderThickness
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

        private int _MaxHeight;

        public int MaxHeight
        {
            get => _MaxHeight;
            set => Set(ref _MaxHeight, value);
        }

        private int _MaxWidth;

        public int MaxWidth
        {
            get => _MaxWidth;
            set => Set(ref _MaxWidth, value);
        }




        int _mul;
        public int MultiplicatorForUiValues { get { return _mul; }
            internal set {
                _mul = value;
                CornerSize = 30 * value;
                BorderThickness = 2 * value;

            } }

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
    }
}
