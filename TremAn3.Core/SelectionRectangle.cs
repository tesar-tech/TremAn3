using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TremAn3.Core
{
    public class SelectionRectangle
    {

        public SelectionRectangle((double X, double Y, double width, double height) value,Color color, double percentageOfResolution = 100)
        {
            RoiModel =  new RoiModel();
            RoiModel.SizeReductionFactor = percentageOfResolution / 100;
            RoiModel.X= (uint)Math.Round(value.X);
            RoiModel.Y = (uint)Math.Round(value.Y);
            RoiModel.Width = (uint)Math.Round(value.width);
            RoiModel.Height = (uint)Math.Round(value.height);
            RoiModel.Color = color;
        }

        public SelectionRectangle(RoiModel model)
        {//From saved file 
            RoiModel = model;
        }

        public RoiModel RoiModel { get; private set; } 

        public uint X { get =>   (uint)Math.Round(RoiModel.X* RoiModel.SizeReductionFactor);  }//its uint here bcsof pixels
        public uint Y { get =>  (uint)Math.Round(RoiModel.Y* RoiModel.SizeReductionFactor);  }
        public uint Width { get => (uint)Math.Round(RoiModel.Width* RoiModel.SizeReductionFactor);  }
        public uint Height { get => (uint)Math.Round(RoiModel.Height* RoiModel.SizeReductionFactor); }

        public bool IsZeroSum { get => X + Y + Width + Height == 0; }

        //this does respect already used percentageOfResolution
        internal  void FullFromResolution(int width, int height)
        {
            RoiModel.Width = (uint)width;
            RoiModel.Height = (uint)height;
        }
    }
}
