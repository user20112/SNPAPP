using DirectShowLib;
using ResumeApp.Classes;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class DirectShowVideoInterface : IVideoSource, ISampleGrabberCB
    {
        public string FriendlyName = "";
        public IBasicVideo ibv = null;
        private readonly Stopwatch FrameWatch = new Stopwatch();
        private int RequestedHeight;
        private int RequestedWidth;
        private IBaseFilter capFilter = null;
        private ISampleGrabber captureSG = null;
        private DsDevice Device;
        private IFilterGraph2 m_FilterGraph = null;
        private IPin m_pinCapture = null;
        private IPin m_pinStill = null;
        private IMediaControl mediaCtrl;
        private IPin pCaptureSampleIn = null;
        private IPin pCaptureSampleOut = null;
        private IPin pRenderIn = null;
        private IPin pStillSampleIn = null;
        private ISampleGrabber stillSampleGrabber = null;

        public DirectShowVideoInterface(string friendlyName, int requestedHeight, int requestedWidth, IntPtr formHandle)
        {
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public unsafe static void CopyZeroEveryFour(IntPtr Source, IntPtr Dest, int length)
        {
            byte* SourcePointer = (byte*)Source;
            byte* DestPointer = (byte*)Dest;
            int x = 0;
            int offset = 0;
            while (x < length)
            {
                DestPointer[x + offset] = SourcePointer[x++];
                DestPointer[x + offset] = SourcePointer[x++];
                DestPointer[x + offset++] = SourcePointer[x++];
                DestPointer[x + offset] = 0;
            }
        }

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (!Paused)
            {
                if (FrameWatch.IsRunning)
                {
                    FrameWatch.Stop();
                    TimeElapsedSinceLastFrame = (int)FrameWatch.ElapsedMilliseconds;
                    FrameWatch.Reset();
                }
                FrameWatch.Start();
                byte[] managedArray = new byte[BufferLen];
                SKBitmap bitmap = new SKBitmap(RequestedWidth, RequestedHeight, SKColorType.Rgb888x, SKAlphaType.Unknown);
                CopyZeroEveryFour(pBuffer, bitmap.GetPixels(), BufferLen);//need to zero out every 4th index as we are converting from RGB24 to RGB32. there is no RGB24 support in SkiaSharp ( RGB88x still uses 4 bytes not three for effeciency just leaveing the fourth a 0.)
                SKImage image = SKImage.FromBitmap(bitmap);
                FrameReady?.Invoke(this, new FrameReadyEventArgs() { FrameBuffer = managedArray, Image = image });
            }
            return 0;
        }

        public void Cleanup()
        {
            if (Device != null)
            {
                Device.Dispose();
                Device = null;
            }
            if (stillSampleGrabber != null)
            {
                Marshal.ReleaseComObject(stillSampleGrabber);
                stillSampleGrabber = null;
            }
            if (captureSG != null)
            {
                Marshal.ReleaseComObject(captureSG);
                stillSampleGrabber = null;
            }
            if (pRenderIn != null)
            {
                Marshal.ReleaseComObject(pRenderIn);
                pRenderIn = null;
            }
            if (pStillSampleIn != null)
            {
                Marshal.ReleaseComObject(pStillSampleIn);
                pStillSampleIn = null;
            }
            if (pCaptureSampleIn != null)
            {
                Marshal.ReleaseComObject(pCaptureSampleIn);
                pCaptureSampleIn = null;
            }
        }

        public override void Dispose()
        {
            Cleanup();
        }

        public override void PauseStream(bool Toggle)
        {
            Paused = Toggle;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            OnMFB?.Invoke(this, new EventArgs());
            return 0;
        }

        public override void StartStream()
        {
            Device = CreateDSDevice(FilterCategory.VideoInputDevice, FriendlyName);
            SetupGraph(Device, RequestedHeight, RequestedWidth, 36);
        }

        public override void StopStream()
        {
            mediaCtrl.Stop();
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Auto)]
        internal static extern int OleCreatePropertyFrame(
        IntPtr hwndOwner,
        uint x, uint y,
        [MarshalAs(UnmanagedType.LPWStr)]
                string caption,
        uint objectCount,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown)]
                object[] lplpUnk,
        int cPages,
        IntPtr pageClsID,
        Guid lcid,
        uint dwReserved,
        IntPtr lpvReserved);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);

        [DllImport("user32.dll")]
        private static extern int GetParent(int hwnd);

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private void ConfigureSampleGrabber(ISampleGrabber sampleGrabber, int bufferCB)
        {
            int hr;
            AMMediaType media = new AMMediaType
            {
                // Set the media type to Video/RBG24
                majorType = MediaType.Video,
                subType = MediaSubType.RGB24,
                formatType = FormatType.VideoInfo
            };
            hr = sampleGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(media);
            // Configure the samplegrabber
            hr = sampleGrabber.SetCallback(this, bufferCB);
            DsError.ThrowExceptionForHR(hr);
        }

        private DsDevice CreateDSDevice(Guid CategoryGUID, string friendlyname)
        {
            Guid BaseFilterGUID = typeof(IBaseFilter).GUID;
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(CategoryGUID))
            {
                if (device.Name.CompareTo(friendlyname) == 0)
                {
                    device.Mon.BindToObject(null, null, ref BaseFilterGUID, out object source);
                    return device;
                }
            }
            return null;
        }

        private IBaseFilter CreateFilter(Guid category, string friendlyname)
        {
            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
            {
                if (device.Name.CompareTo(friendlyname) == 0)
                {
                    device.Mon.BindToObject(null, null, ref iid, out source);
                    break;
                }
            }
            return (IBaseFilter)source;
        }

        private void SetPinParameters(IPin pin, int iWidth, int iHeight, short iBPP)
        {
            int hr;
            IAMStreamConfig videoStreamConfig = pin as IAMStreamConfig;
            hr = videoStreamConfig.GetFormat(out AMMediaType media);
            DsError.ThrowExceptionForHR(hr);
            try
            {
                VideoInfoHeader v = new VideoInfoHeader();
                Marshal.PtrToStructure(media.formatPtr, v);
                if (iWidth > 0)
                {
                    v.BmiHeader.Width = iWidth;
                }
                if (iHeight > 0)
                {
                    v.BmiHeader.Height = iHeight;
                }
                if (iBPP > 0)
                {
                    v.BmiHeader.BitCount = iBPP;
                }
                media.majorType = MediaType.Video;
                media.subType = MediaSubType.MJPG;
                Marshal.StructureToPtr(v, media.formatPtr, false);
                hr = videoStreamConfig.SetFormat(media);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                DsUtils.FreeAMMediaType(media);
            }
        }

        private void SetupCapturePin(int RequestedHeight, int RequestedWidth, short iBPP)
        {
            int hr;
            if (RequestedHeight + RequestedWidth + iBPP > 0)
            {
                SetPinParameters(m_pinCapture, RequestedWidth, RequestedHeight, iBPP);
            }
            captureSG = new SampleGrabber() as ISampleGrabber;
            IBaseFilter baseCaptureGrabFlt = captureSG as IBaseFilter;
            ConfigureSampleGrabber(captureSG, 1);
            pCaptureSampleIn = DsFindPin.ByDirection(baseCaptureGrabFlt, PinDirection.Input, 0);
            hr = m_FilterGraph.AddFilter(baseCaptureGrabFlt, "Ds.NET Grabber");
            DsError.ThrowExceptionForHR(hr);
            hr = m_FilterGraph.Connect(m_pinCapture, pCaptureSampleIn);
            DsError.ThrowExceptionForHR(hr);
            pCaptureSampleOut = DsFindPin.ByDirection(baseCaptureGrabFlt, PinDirection.Output, 0);
            //SetupDummyWindow();
        }

        private void SetupDummyWindow()
        {
            Guid CLSID_VideoMixingRenderer9 = new Guid("{51B4ABF3-748F-4E3B-A276-C828330E926A}"); //quartz.dll
            IBaseFilter pRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_VideoMixingRenderer9));
            m_FilterGraph.AddFilter(pRenderer, "Video Mixing Renderer 9");
            pRenderIn = DsFindPin.ByDirection(pRenderer, PinDirection.Input, 0);
            m_FilterGraph.Connect(pCaptureSampleOut, pRenderIn);
            ibv = pRenderer as IBasicVideo;
        }

        private void SetupGraph(DsDevice dev, int RequestedHeight, int RequestedWidth, short iBPP)
        {
            int hr;
            m_FilterGraph = new FilterGraph() as IFilterGraph2;
            try
            {
                hr = m_FilterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
                DsError.ThrowExceptionForHR(hr);
                m_pinCapture = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);
                SetupCapturePin(RequestedHeight, RequestedWidth, iBPP);
                SetupStillPin(RequestedHeight, RequestedWidth, iBPP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Setup Graph Error: " + ex.Message);
            }
        }

        private void SetupStillPin(int RequestedHeight, int RequestedWidth, short iBPP)
        {
            int hr;
            if (RequestedHeight + RequestedWidth + iBPP > 0)
            {
                SetPinParameters(m_pinStill, RequestedWidth, RequestedHeight, iBPP);
            }
            stillSampleGrabber = new SampleGrabber() as ISampleGrabber;
            IBaseFilter baseStillGrabFlt = stillSampleGrabber as IBaseFilter;
            ConfigureSampleGrabber(stillSampleGrabber, 0);
            pStillSampleIn = DsFindPin.ByDirection(baseStillGrabFlt, PinDirection.Input, 0);
            hr = m_FilterGraph.AddFilter(baseStillGrabFlt, "Ds.NET Grabber");
            DsError.ThrowExceptionForHR(hr);
            hr = m_FilterGraph.Connect(m_pinStill, pStillSampleIn);
            DsError.ThrowExceptionForHR(hr);
            hr = ((ISampleGrabber)stillSampleGrabber).SetOneShot(true);
            DsError.ThrowExceptionForHR(hr);
            AMMediaType media = new AMMediaType();
            hr = stillSampleGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);
            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }
            DsUtils.FreeAMMediaType(media);
            mediaCtrl = m_FilterGraph as IMediaControl;
            hr = mediaCtrl.Run();
            DsError.ThrowExceptionForHR(hr);
        }

        public override void Load(string FriendlyName, int FrameRate, int Height, int Width, string Encoding)
        {
            RequestedHeight = Height;
            RequestedWidth = Width;
            this.FriendlyName = FriendlyName;
        }

        public override List<VideoType> ScanDevice(string friendlyName)
        {
            throw new NotImplementedException();
        }

        public override List<string> Scan()
        {
            throw new NotImplementedException();
        }
    }
}