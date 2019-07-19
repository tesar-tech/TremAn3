using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TremAn3.Services
{
    public class FramesGrabber
    {

        public FramesGrabber()
        {

        }


        private async Task GetVideoPropertiesAsync()
        {
            VideoProperties videoProperties = await StorageFile.Properties.GetVideoPropertiesAsync();
            IDictionary<string, object> encodingProperties = await videoProperties.RetrievePropertiesAsync(new List<string> { "System.Video.FrameRate" });
            uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
            frameRate = frameRateX1000 / 1000d;
            videoHeight = videoProperties.Height;
            videoWidth = videoProperties.Width;
            duration = videoProperties.Duration;
        }
        uint videoHeight;
        uint videoWidth;

        public (int width, int height) GetWidthAndHeight()
        {
            return ((int)videoWidth, (int)videoHeight);
        }

        internal async Task ChangeStorageFileAsync(StorageFile file)
        {
            StorageFile = file;
            await GetVideoPropertiesAsync();
            var clip = await MediaClip.CreateFromFileAsync(StorageFile);
             composition = new MediaComposition();
            composition.Clips.Add(clip);

             framePeriod = 1 / frameRate;
             var framesGaps = (int)Math.Round(duration.TotalSeconds * frameRate);
             frameTimes = Enumerable.Range(0, framesGaps-1).Select(x => TimeSpan.FromSeconds(framePeriod * x)).ToList();
            //-1 is because there is no frame (but there should be), and it throws error about expected range (in generate thumbnail)
            //if (frameTimes.Last() < duration)
            //    frameTimes.Add(duration);
            framesCount = frameTimes.Count;
            FrameIndex = 0;
        }
        double framePeriod;
        int framesCount;
        List<TimeSpan> frameTimes;

        MediaComposition composition;

        TimeSpan duration; 
        double frameRate;

        public StorageFile   StorageFile { get; set; }
        public TimeSpan Position { get; set; }

        public int FrameIndex { get; set; }
        //enable to grab frames in portions instead of one by one 
        IEnumerator<ImageStream> currentEnumerator;
        public int batchSize = 1;
        // returns null if there is no frame left
        public async Task<byte[]> GrabGrayFrameInCurrentIndexAsync()
        {
            //get new portion of frames if there is nothing left.
            if (currentEnumerator == null || !currentEnumerator.MoveNext())
            {
                var timesToFrames = frameTimes.Skip(FrameIndex + 1).Take(batchSize);
                if (timesToFrames.Count() == 0)
                    return null;
                //get next portion of frames

                var imageStreams = await composition.GetThumbnailsAsync(timesToFrames, 0, 0, VideoFramePrecision.NearestFrame);
                //if the video resolution is not even- it will generate even resolution and adds black stripe
                // why? dont know 
                currentEnumerator = imageStreams.GetEnumerator();
                if (!currentEnumerator.MoveNext())
                    return null;
            }
        
            ImageStream imageStream = currentEnumerator.Current;


            var cosi = await BitmapDecoder.CreateAsync(imageStream);
                var pixelss = await cosi.GetPixelDataAsync();
                var bytes = pixelss.DetachPixelData();
                // bytes is array with size = viedeoWidth * video Height * 4 ; (4 because of R G B and alpha).
                //bytes[0] - R value of first pixel, bytes[1] - Green value of first pixel...
                var grayBytes = new byte[bytes.Length / 4];
                int grayBytrsIterator = 0;
                for (int i = 0; i < bytes.Length; i += 4)
                {
                    grayBytes[grayBytrsIterator] = (byte)(0.2989 * bytes[i] + 0.5870 * bytes[i + 1] + 0.1140 * bytes[i + 2]);
                    grayBytrsIterator++;
                }
                FrameIndex++;
                return grayBytes;
        }


        public bool MovePositionToNextFrame()
        {
            FrameIndex += 1;
            if (FrameIndex + 1 >= frameTimes.Count)
                return false;
            return true;
        }




        //int framesCount = (int)Math.Round(duration.TotalSeconds * frameRate);
        //var framePeriod = 1 / frameRate;
        //var frameTimes = Enumerable.Range(1, framesCount - 1).Select(x => TimeSpan.FromSeconds(framePeriod * x));

        //var imagestream = await composition.GetThumbnailsAsync(frameTimes, (int)videoWidth, (int)videoHeight, VideoFramePrecision.NearestFrame);

        //        foreach (var tim in frameTimes)
        //        {
    }
}
