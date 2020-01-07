using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage.FileProperties;
using Windows.Media.MediaProperties;
using Windows.Graphics.DirectX.Direct3D11;
using System.Diagnostics;

namespace FrameGrabberProj
{
    public sealed class FrameGrabberEffect : IBasicVideoEffect
    {
        public FrameGrabberEffect()
        {

        }


        public void ProcessFrame(ProcessVideoFrameContext context)
        {
            //Getingt pixel data

            var inputFrameBitmap = context.InputFrame.SoftwareBitmap;
            var frameSize = inputFrameBitmap.PixelWidth * inputFrameBitmap.PixelHeight * 4;

            var frameBuffer = new Windows.Storage.Streams.Buffer((uint)frameSize);
            context.InputFrame.SoftwareBitmap.CopyToBuffer(frameBuffer);
            var framePixels = frameBuffer.ToArray();

            Debug.WriteLine($"Touching 25th red pixel {framePixels[100]}");
        }
        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device)
        {
        }

        public void Close(MediaEffectClosedReason reason)
        {

        }

        public void DiscardQueuedFrames()
        {
        }

        public bool IsReadOnly { get; }
        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties { get; } = new List<VideoEncodingProperties>();
        public MediaMemoryTypes SupportedMemoryTypes { get; } = MediaMemoryTypes.Cpu;// MediaMemoryTypes.GpuAndCpu
        public bool TimeIndependent { get; } = true;

        public void SetProperties(IPropertySet configuration)
        {
        }

      

        IReadOnlyList<VideoEncodingProperties> IBasicVideoEffect.SupportedEncodingProperties { get; }

        
    }
}
