using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.ViewModels;

namespace TremAn3.Services
{
    public class StoringMeasurementsService
    {
        public void DisplayMeasurementByModel(MeasurementModel measurementModel)
        {

            var mainVm = ViewModelLocator.Current.MainViewModel;
            mainVm.CurrentResultsViewModel = new ResultsViewModel();
            mainVm.CurrentResultsViewModel.CoherenceResult = measurementModel.Coherence;



             mainVm.MediaPlayerViewModel.MediaControllingViewModel.PositionSeconds = measurementModel.PositionSeconds;
            mainVm.MediaPlayerViewModel.FreqCounterViewModel.Maxrange = measurementModel.Maxrange;
            mainVm.MediaPlayerViewModel.FreqCounterViewModel.Minrange = measurementModel.Minrange;
            ViewModelLocator.Current.DrawingRectanglesViewModel.RemoveRois();
            foreach (var roiRes in measurementModel.RoiResultModels)
            {
                //this creates roi on video
                var selvm = ViewModelLocator.Current.DrawingRectanglesViewModel.CreateROIFromModel(roiRes);

                //roi has alg inside, this alg hold data for plots
                selvm.ComputationViewModel.Algorithm = new CenterOfMotionAlgorithm(roiRes, measurementModel.FrameRate);
            }
            mainVm.FreqCounterViewModel.DisplayPlots();

        }

        public void DeleteStoredModel(MeasurementModel measurementModel)
        {

        }
    }
}
