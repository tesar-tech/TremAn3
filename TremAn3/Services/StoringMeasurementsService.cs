using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TremAn3.Core;
using TremAn3.ViewModels;
using Windows.UI.Core;

namespace TremAn3.Services
{
    public class MeasurementsService
    {

        public MeasurementsService(DataService ds)
        {
            _DataService = ds;
        }
        DataService _DataService;
        /// <summary>
        /// this is called when model is loaded from file and we want display rresults from model.
        /// here the data gets from model to vm
        /// </summary>
        /// <param name="measurementModel"></param>
        /// <returns></returns>
        public async Task DisplayMeasurementByModelAsync(MeasurementModel measurementModel)
        {

            var mainVm = ViewModelLocator.Current.MainViewModel;

            mainVm.MediaPlayerViewModel.MediaControllingViewModel.PositionSeconds = measurementModel.PositionSeconds;
            mainVm.MediaPlayerViewModel.FreqCounterViewModel.Maxrange = measurementModel.Maxrange;
            mainVm.MediaPlayerViewModel.FreqCounterViewModel.Minrange = measurementModel.Minrange;

            mainVm.MediaPlayerViewModel.FreqCounterViewModel.FreqProgressViewModel.SegmnetSize = measurementModel.FreqProgressSegmnetSize;

            ViewModelLocator.Current.DrawingRectanglesViewModel.RemoveRois();
            foreach (var roiRes in measurementModel.VectorsDataModel.RoiResultModels)
            {
                //this creates roi on video
                var selvm = ViewModelLocator.Current.DrawingRectanglesViewModel.CreateROIFromModel(roiRes);

                //roi has alg inside, this alg hold data for plots
                selvm.ComputationViewModel.Algorithm = new CenterOfMotionAlgorithm(roiRes, measurementModel.FrameRate);
            }
            ResultsViewModel rvm = mainVm.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel;

            //collectors
            //rvm.PointsCollectors.Clear(); <-- dont do this
            foreach (var collectorModel in measurementModel.AdditionalResultsModel.PointsCollectors)
            {
                var collectorVm = rvm.PointsCollectors[collectorModel.DataSeriesType];
                collectorVm.Points.Clear();
                collectorModel.Points.ForEach(x => collectorVm.Points.Add(new PointToCollectVm(x.X,x.Ys ,collectorVm)));
            }


            //data results
            rvm.DataResultsDict.Clear();

            foreach (var gsdm in measurementModel.VectorsDataModel.GlobalScopedDataResultsModels)
            {
                DataResult dr = new()
                {
                    Y = gsdm.Y,
                    X = gsdm.X,
                    ErrorMessage = gsdm.ErrorMessage
                };
                rvm.DataResultsDict.Add(gsdm.DataSeriesType, dr);
            }

            //coherence
            rvm.CoherenceMeasurementResults.Clear();
            foreach (var item in measurementModel.AdditionalResultsModel.CoherenceAverageResults)
                rvm.CoherenceMeasurementResults.Add(new CoherenceMeasurementResults() { Average = item.Average, MaxHz = item.MaxHz, MinHz = item.MinHz });

            
            if (rvm.CoherenceMeasurementResults.Any())
            {
                rvm.CoherenceMinHz = rvm.CoherenceMeasurementResults.Last().MinHz;
                rvm.CoherenceMaxHz = rvm.CoherenceMeasurementResults.Last().MaxHz;
            }
            else
            {
                rvm.CoherenceMinHz = 0;
                rvm.CoherenceMaxHz = mainVm.MediaPlayerViewModel.VideoPropsViewModel.FrameRate / 2;
            }


            rvm.SetIsCoherenceOk();
            rvm.IsComputationPaused = false;

            await mainVm.FreqCounterViewModel.DisplayPlots(false);

        }
        /// <summary>
        /// new measurement is done, it is written inside vm and this method will add it to the model. Also returns folder for future work (for example renaming)
        /// Model is created elswere since it is more convinient..
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public async Task GetModelFromVmAndSaveItToFile(MeasurementViewModel vm)
        {
            var mainVm = ViewModelLocator.Current.MainViewModel;
            var algs = ViewModelLocator.Current.DrawingRectanglesViewModel.SelectionRectanglesViewModels.Select(x => x.ComputationViewModel.Algorithm);


            var currentGlobalResults = mainVm.FreqCounterViewModel.CurrentGlobalScopedResultsViewModel;
            vm.Model.GetResults(algs, currentGlobalResults.DataResultsDict, currentGlobalResults.GetAdditionalResultsModel());
            vm.Model.FreqProgressSegmnetSize = mainVm.FreqCounterViewModel.FreqProgressViewModel.SegmnetSize;
            vm.Model.FreqProgressStep = mainVm.FreqCounterViewModel.FreqProgressViewModel.Step;
            if (vm.FolderForMeasurement == null)
                vm.FolderForMeasurement = await _DataService.SaveMeasurementResults(vm.Model, mainVm.MediaPlayerViewModel.VideoFileModel);
            else
                await DataService.SaveMeasurementResults(vm.Model, vm.FolderForMeasurement);
        }



    }
}
