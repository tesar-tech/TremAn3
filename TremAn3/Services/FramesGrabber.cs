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
using TremAn3.ViewModels;

namespace TremAn3.Services
{
    public class FramesGrabber
    {
        //ctors
        public static async Task<FramesGrabber> CtorAsync(StorageFile currentStorageFile, VideoPropsViewModel videoPropsViewModel, double perc, TimeSpan start, TimeSpan end)
        {
            //how to make ctor async: https://stackoverflow.com/questions/8145479/can-constructors-be-async
            var thisObj = new FramesGrabber(currentStorageFile, videoPropsViewModel, perc,start,end);
            await thisObj.InitAsync();
            return thisObj;
        }
      
        private FramesGrabber(StorageFile currentStorageFile, VideoPropsViewModel videoPropsViewModel, double perc,TimeSpan start, TimeSpan end) 
        {
            //we can do this in InitAsync, but loosing readonly modifiers...
            file = currentStorageFile;
            this.videoPropsViewModel = videoPropsViewModel;
            this.start = start;
            this.end = end;
            framesCount = (int)Math.Round(RangeDuration.TotalSeconds* videoPropsViewModel.FrameRate);
            percentageOfResolution = perc;
        }

        private async Task InitAsync()
        {
            var stream = await file.OpenAsync(FileAccessMode.Read);
            grabber = await FrameGrabber.CreateFromStreamAsync(stream);
            
            DecodedPixelWidth=(int)Math.Round(videoPropsViewModel.Width * percentageOfResolution / 100);
            DecodedPixelHeight = (int)Math.Round(videoPropsViewModel.Height * percentageOfResolution / 100);

            grabber.DecodePixelWidth = DecodedPixelWidth;
            grabber.DecodePixelHeight = DecodedPixelHeight;
        }
        //ctors
    
    

        FrameGrabber grabber;
        readonly StorageFile file;
        readonly VideoPropsViewModel videoPropsViewModel;
        readonly double percentageOfResolution;
        readonly int framesCount;
        readonly TimeSpan start;
        readonly TimeSpan end;
        int frameIndex = 0;

        public TimeSpan RangeDuration { get=> end-start;  }
        public int DecodedPixelWidth { get;private set; }
        public int DecodedPixelHeight { get;private set; }

        public double ProgressPercentage { get =>  (double)(frameIndex) / framesCount * 100;  }
        
        public async Task<byte[]> GrabARGBFrameInCurrentIndexAsync()
        {
            VideoFrame frame;
            if (frameIndex == 0)
                frame = await grabber.ExtractVideoFrameAsync(start);
            else
                frame = await grabber.ExtractNextVideoFrameAsync();
            if (frame == null || frame.Timestamp > end)//it is last frame or frame we dont want to
                return null;
            var data =  frame.PixelData.ToArray();
            //var grayBytes = new byte[data.Length / 4];
            //int grayBytrsIterator = 0;
            //for (int i = 0; i < data.Length; i += 4)
            //{
            //    grayBytes[grayBytrsIterator] = (byte)(0.2989 * data[i] + 0.5870 * data[i + 1] + 0.1140 * data[i + 2]);
            //    grayBytrsIterator++;
            //}
            frameIndex++;
            return data;
        }
    }
}
