using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TremAn3.Core.Helpers
{
    public class ColorHelper
    {

        public static Color GetNextColorThatIsNotInList(List<Color> colorsList)
        {
            List<Color> colors = new List<Color>()
                {
            Color.Red,Color.Green,Color.Blue,Color.Cyan,Color.Magenta, Color.Yellow,Color.CornflowerBlue, Color.SpringGreen
                };
            foreach (var col in colors)
                if (!colorsList.Contains(col))//return 1st color that is not in list.
                    return col;
            Random rnd = new Random();
            return Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));

        }


    }
}
