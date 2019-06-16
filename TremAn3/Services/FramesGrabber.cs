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

        internal async Task ChangeStorageFileAsync(StorageFile file)
        {
            StorageFile = file;
            await GetVideoPropertiesAsync();
            var clip = await MediaClip.CreateFromFileAsync(StorageFile);
             composition = new MediaComposition();
            composition.Clips.Add(clip);
        }
        MediaComposition composition;

        TimeSpan duration; 
        double frameRate;

        public StorageFile   StorageFile { get; set; }
        public TimeSpan Position { get; set; }

        public async Task GrabGrayFrameInPositionAsync()
        {
          
            var imageStream = await composition.GetThumbnailAsync(Position, (int)videoWidth, (int)videoHeight, VideoFramePrecision.NearestFrame);
            var cosi = await BitmapDecoder.CreateAsync(imageStream);
            var pixelss = await cosi.GetPixelDataAsync();
            var bytes = pixelss.DetachPixelData();
            // bytes is array with size = viedeoWidth * video Height * 4 ; (4 because of R G B and alpha).
            //bytes[0] - R value of first pixel, bytes[1] - Green value of first pixel...
            var grayBytes = new byte[bytes.Length / 4];
            int grayBytrsIterator = 0;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                grayBytes[grayBytrsIterator] = (byte)(0.2989*bytes[i] + 0.5870*bytes[i + 1] + 0.1140* bytes[i + 2]);
                    grayBytrsIterator++;
            }
            
        }



    //int framesCount = (int)Math.Round(duration.TotalSeconds * frameRate);
    //var framePeriod = 1 / frameRate;
    //var frameTimes = Enumerable.Range(1, framesCount - 1).Select(x => TimeSpan.FromSeconds(framePeriod * x));

    //var imagestream = await composition.GetThumbnailsAsync(frameTimes, (int)videoWidth, (int)videoHeight, VideoFramePrecision.NearestFrame);

    //        foreach (var tim in frameTimes)
    //        {
}
}
