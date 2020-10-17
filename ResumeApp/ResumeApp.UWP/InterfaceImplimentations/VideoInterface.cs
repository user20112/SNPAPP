using ResumeApp.Classes;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using ResumeApp.UWP.InterfaceImplimentations;
using ResumeApp.UWP.PlatformSpecific;
using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.UI.Xaml.Controls;

[assembly: Xamarin.Forms.Dependency(typeof(VideoSource))]

namespace ResumeApp.UWP.PlatformSpecific
{
    public class VideoSource : IVideoSource
    {
        private readonly Stopwatch StreamFaultTimer = new Stopwatch();
        private MediaCapture NativeDevice;
        private bool paused = false;
        private MediaFrameReader Reader;
        private bool Streaming = false;

        public override void Dispose()
        {
            Streaming = false;
            if (Reader != null)
            {
                Reader.FrameArrived -= NewFrameArrived;
                Reader.Dispose();
            }
            if (NativeDevice != null)
                NativeDevice.Dispose();
        }

        public void HandleFrame(SKImage frame)
        {
            if (!paused)
                FrameReady?.Invoke(this, new FrameReadyEventArgs() { FrameBuffer = null, Image = frame });
        }

        public override void Load(string FriendlyName, int FrameRate, int Height, int Width, string Encoding)
        {
            if (NativeDevice != null)
                NativeDevice.Dispose();
            NativeDevice = new MediaCapture();
            var task = NativeDevice.InitializeAsync(new MediaCaptureInitializationSettings() { VideoDeviceId = FriendlyName, MemoryPreference = MediaCaptureMemoryPreference.Auto, StreamingCaptureMode = StreamingCaptureMode.Video });
            while (task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            if (task.Status == AsyncStatus.Error)
                throw new System.Exception("Access Denied");
            if (Reader != null)
            {
                Reader.FrameArrived -= NewFrameArrived;
                Reader.Dispose();
            }
            IReadOnlyDictionary<string, MediaFrameSource> sources = NativeDevice.FrameSources;
            MediaFrameSource selectedSource = null;
            foreach (MediaFrameSource source in sources.Values)
            {
                if (source.CurrentFormat.MajorType == "Video")
                {
                    if (source.Info.MediaStreamType == MediaStreamType.VideoPreview || source.Info.MediaStreamType == MediaStreamType.VideoRecord)
                    {
                        foreach (MediaFrameFormat format in source.SupportedFormats)
                        {
                            if (format.VideoFormat.Height == 480 && format.VideoFormat.Width == 640 && format.VideoFormat.MediaFrameFormat.Subtype == "MJPG")
                            {
                                if (selectedSource == null)
                                {
                                    selectedSource = source;
                                    var SetTastk = selectedSource.SetFormatAsync(format);
                                    while (SetTastk.Status == AsyncStatus.Started)
                                        Thread.Sleep(50);
                                }
                            }
                        }
                    }
                }
            }
            var Task = NativeDevice.CreateFrameReaderAsync(selectedSource);
            while (Task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            Reader = Task.GetResults();
        }

        public override void PauseStream(bool Toggle)
        {
            paused = Toggle;
        }

        public override List<string> Scan()
        {
            List<string> found = new List<string>();
            IAsyncOperation<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            while (task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            DeviceInformationCollection videoDevices = task.GetResults();
            foreach (DeviceInformation device in videoDevices)
            {
                found.Add(device.Id);
            }
            return found;
        }

        public override List<VideoType> ScanDevice(string friendlyName)
        {
            List<VideoType> found = new List<VideoType>();
            IAsyncOperation<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            while (task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            DeviceInformationCollection videoDevices = task.GetResults();
            foreach (DeviceInformation device in videoDevices)
            {
                if (device.Name.Contains(friendlyName))
                {
                    found.Add(new VideoType(device.Id, 30, 640, 480, "Mjpeg"));
                }
            }
            return found;
        }

        public override void StartStream()
        {
            paused = false;
            if (!Streaming)
            {
                Streaming = true;
                Reader.FrameArrived += NewFrameArrived;
                var task = Reader.StartAsync();
                while (task.Status == AsyncStatus.Started)
                    Thread.Sleep(50);
                CaptureElement captureElement = null;
                bool waiting = true;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    captureElement = new CaptureElement
                    {
                        Source = NativeDevice
                    };
                    waiting = false;
                });
                NativeDevice.Failed += OnFail;
                while (waiting)
                    Thread.Sleep(50);
                Task.Run(() =>
                {
                    while (Streaming)
                    {
                        if (StreamFaultTimer.ElapsedMilliseconds > 1000)
                        {
                            Debug.WriteLine("Been a second without frames. forcing a call");
                            var Task = Reader.StopAsync();
                            while (Task.Status == AsyncStatus.Started)
                                Thread.Sleep(50);
                            var taskt = Reader.StartAsync();
                            while (taskt.Status == AsyncStatus.Started)
                                Thread.Sleep(50);
                            StreamFaultTimer.Reset();
                        }
                    }
                });
            }
        }

        public override void StopStream()
        {
            Streaming = false;
            if (Reader != null)
            {
                var Task = NativeDevice.StopPreviewAsync();
                while (Task.Status == AsyncStatus.Started)
                    Thread.Sleep(50);
                //var task = Reader.StopAsync();
                //while (task.Status == AsyncStatus.Started)
                //    Thread.Sleep(50);
                Reader.FrameArrived -= NewFrameArrived;
            }
        }

        private void NewFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (!StreamFaultTimer.IsRunning)
                StreamFaultTimer.Start();
            StreamFaultTimer.Restart();
            MediaFrameReference LatestFrame = sender.TryAcquireLatestFrame();
            if (LatestFrame != null)
            {
                VideoMediaFrame LatestVideoFrame = LatestFrame.VideoMediaFrame;
                if (LatestVideoFrame.SoftwareBitmap == null)
                    HandleFrame(Convert.Direct3dToSKImage(LatestVideoFrame.Direct3DSurface));
                else
                    HandleFrame(Convert.SoftwareBitmapToSKImage(LatestVideoFrame.SoftwareBitmap));
                if (LatestVideoFrame.Direct3DSurface != null)
                    LatestVideoFrame.Direct3DSurface.Dispose();
                if (LatestVideoFrame.SoftwareBitmap != null)
                    LatestVideoFrame.SoftwareBitmap.Dispose();
                LatestFrame.Dispose();
            }
            else
            {
            }
        }

        private void OnFail(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
        }
    }
}