using System;
using System.Collections.Generic;
using System.Text;

namespace TremAn3.Core
{
    public class SelectionRectangle
    {

        public SelectionRectangle((uint X, uint Y, uint width, uint height) value, double percentageOfResolution = 100)
        {
            sizeReductionFactor = percentageOfResolution / 100;
            x = value.X;
            y = value.Y;
            width= value.width;
            height = value.height;
        }

        private double sizeReductionFactor;
        private uint x;
        private uint y;
        private uint width;
        private uint height;

        public uint X { get =>   (uint)Math.Round( x * sizeReductionFactor);  set => x = value; }
        public uint Y { get =>  (uint)Math.Round(y * sizeReductionFactor); set => y = value; }
        public uint Width { get => (uint)Math.Round(width * sizeReductionFactor); set => width = value; }
        public uint Height { get => (uint)Math.Round(height * sizeReductionFactor); set => height = value; }

        public bool IsZeroSum { get => X + Y + Width + Height == 0; }

        //this does respect already used percentageOfResolution
        internal  void FullFromResolution(int width, int height)
        {
            Width = (uint)width;
            Height = (uint)height;
        }
    }
}
