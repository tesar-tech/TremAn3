using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace TremAn3.ViewModels
{
    public partial class MainViewModel
    {
        public class PlotModelDefine
        {
            public static PlotModel Fftplot()
            {
                var plotModel = new PlotModel { Title = "fft" };
                plotModel.Series.Add(new FunctionSeries(System.Math.Sin, 0, 10, 0.1, "sin(x)"));
                plotModel.PlotAreaBorderThickness = new OxyThickness(0.1);
                LinearAxis xAxis = new LinearAxis();
                LinearAxis yAxis = new LinearAxis();
                xAxis.Maximum = 10;
                xAxis.Minimum = -10;
                xAxis.PositionAtZeroCrossing = true;
                yAxis.Maximum = 10;
                yAxis.Minimum = 0;
                yAxis.PositionAtZeroCrossing = true;
                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);
                return plotModel;
            }
        }

    }
}
