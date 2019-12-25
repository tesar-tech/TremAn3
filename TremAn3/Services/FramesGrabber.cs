using FFmpegInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

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
            FrameRate = frameRateX1000 / 1000d;
            videoHeight = videoProperties.Height;
            videoWidth = videoProperties.Width;
            duration = videoProperties.Duration;
            //frameRate = 25;
            //videoHeight = 288;
            //videoWidth = 512;
            //duration = TimeSpan.FromSeconds(10);
        }
        uint videoHeight;
        uint videoWidth;

        public double FrameRate { get; set; }

        public (int width, int height) GetWidthAndHeight()
        {
            return ((int)videoWidth, (int)videoHeight);
        }

        internal async Task ChangeStorageFileAsync(StorageFile file)
        {
            StorageFile = file;
            await GetVideoPropertiesAsync();
            //var clip = await MediaClip.CreateFromFileAsync(StorageFile);
            // composition = new MediaComposition();
            //composition.Clips.Add(clip);

            // framePeriod = 1 / FrameRate;
            // var framesGaps = (int)Math.Round(duration.TotalSeconds * FrameRate);
            // frameTimes = Enumerable.Range(0, framesGaps-1).Select(x => TimeSpan.FromSeconds(framePeriod * x)).ToList();
            ////-1 is because there is no frame (but there should be), and it throws error about expected range (in generate thumbnail)
            ////if (frameTimes.Last() < duration)
            ////    frameTimes.Add(duration);
            framesCount = (int)Math.Round( duration.TotalSeconds * FrameRate);
            FrameIndex = 0;
            var stream = await StorageFile.OpenAsync(FileAccessMode.Read);
            grabber = await FrameGrabber.CreateFromStreamAsync(stream);
        }
        double framePeriod;
        FrameGrabber grabber;

        internal double GetProgressPercentage()
        {
            return (double)(FrameIndex) / framesCount *100;
        }

        int framesCount;
        List<TimeSpan> frameTimes;

        MediaComposition composition;

        TimeSpan duration; 

        public StorageFile   StorageFile { get; set; }
        public TimeSpan Position { get; set; }

        public int FrameIndex { get; set; }
        //enable to grab frames in portions instead of one by one 
        IEnumerator<ImageStream> currentEnumerator;
        public int batchSize = 1;
        // returns null if there is no frame left
        IReadOnlyList<ImageStream> imgsStreams;
        public async Task<byte[]> GrabGrayFrameInCurrentIndexAsync()
        {
            VideoFrame frame = null;
            if (FrameIndex == 0)
                frame = await grabber.ExtractVideoFrameAsync(TimeSpan.Zero);
            else
                frame = await grabber.ExtractNextVideoFrameAsync();
            if (frame == null)
                return null;
            var data =  frame.PixelData.ToArray();
            var grayBytes = new byte[data.Length / 4];
            int grayBytrsIterator = 0;
            for (int i = 0; i < data.Length; i += 4)
            {
                grayBytes[grayBytrsIterator] = (byte)(0.2989 * data[i] + 0.5870 * data[i + 1] + 0.1140 * data[i + 2]);
                grayBytrsIterator++;
            }
            FrameIndex++;
            return grayBytes;
            //get new portion of frames if there is nothing left.
            //if (currentEnumerator == null || !currentEnumerator.MoveNext())
            //{
            //    var timesToFrames = frameTimes.Skip(FrameIndex + 1).Take(batchSize);
            //    if (timesToFrames.Count() == 0)
            //        return null;
            //    //get next portion of frames

            //    var imageStreams = await composition.GetThumbnailsAsync(timesToFrames, (int)videoWidth, (int)videoHeight, VideoFramePrecision.NearestFrame);
            //    //if the video resolution is not even- it will generate even resolution and adds black stripe
            //    // why? dont know 
            //    currentEnumerator = imageStreams.GetEnumerator();
            //    if (!currentEnumerator.MoveNext())
            //        return null;
            //}

            //if(imgsStreams == null)
            //    {

            //     frameTimes = Enumerable.Range(0, 299).Select(x => TimeSpan.FromSeconds(framePeriod * x)).ToList();

            //    imgsStreams = await composition.GetThumbnailsAsync(frameTimes, 0,0, VideoFramePrecision.NearestFrame);
            //}
            if (FrameIndex >= frameTimes.Count)
                return null;
            //var isss = await composition.GetThumbnailAsync(frameTimes[FrameIndex], (int)videoWidth, (int)videoHeight, VideoFramePrecision.NearestFrame);

            //ImageStream imageStream = isss;
            //ImageStream imageStream = currentEnumerator.Current;
            //ImageStream imageStream = imgsStreams[FrameIndex];
            var imageStreamsSingle = await composition.GetThumbnailAsync(frameTimes[FrameIndex], 0, 0, VideoFramePrecision.NearestFrame);



                //var bytes = await GetBytesFromImageStreamAsync(imageStream);
                var bytes = await GetBytesFromImageStreamAsync(imageStreamsSingle);
            //var absDiff = bytes.Zip(bytesSingle, (m, s) => Math.Abs(m - s)).Sum();
            //if (absDiff != 0)
            //{
            //    Debug.WriteLine(absDiff);

            //}
            // bytes is array with size = viedeoWidth * video Height * 4 ; (4 because of R G B and alpha).
            //bytes[0] - R value of first pixel, bytes[1] - Green value of first pixel...
            
        }

        async System.Threading.Tasks.Task<byte[]> GetBytesFromImageStreamAsync(ImageStream stream)
        {
            var deco = await BitmapDecoder.CreateAsync(stream);
            var pixelss = await deco.GetPixelDataAsync();
            var bytes = pixelss.DetachPixelData();
            return bytes;
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
