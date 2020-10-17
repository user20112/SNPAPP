using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using ResumeApp.Classes;
using ResumeApp.Droid.CallBacks;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using SkiaSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(ResumeApp.Droid.PlatformSpecific.VideoSource))]

namespace ResumeApp.Droid.PlatformSpecific
{
    public class VideoSource : IVideoSource
    {
        public CameraCaptureSessionCallback callback;
        public Context context;
        public CameraStateListener listener;
        public SynchronizationContext Main;
        public CameraManager manager;
        public Handler mBackgroundHandler;
        public HandlerThread mBackgroundThread;
        public CameraDevice mCameraDevice;
        public CaptureRequest mPreviewRequest;
        public CaptureRequest.Builder mPreviewRequestBuilder;
        public ImageReader multiReader;
        public CameraCaptureSession Session;
        public CameraCaptureSessionStateCallback StateCallback;
        public int DegreesToRotateImage = 0;
        private string CameraID = "";
        private ImageReaderCallback ImageReaderCallback;
        public Dictionary<SurfaceOrientation, int> OrientationToDegrees = new Dictionary<SurfaceOrientation, int>();

        public VideoSource()
        {
            OrientationToDegrees.Add(SurfaceOrientation.Rotation0, 90);
            OrientationToDegrees.Add(SurfaceOrientation.Rotation90, 0);
            OrientationToDegrees.Add(SurfaceOrientation.Rotation180, 270);
            OrientationToDegrees.Add(SurfaceOrientation.Rotation270, 180);
            Main = SynchronizationContext.Current;
            context = MainActivity.Instance;
            manager = (CameraManager)context.GetSystemService(Context.CameraService);
            listener = new CameraStateListener(this);
            callback = new CameraCaptureSessionCallback(this);
            StateCallback = new CameraCaptureSessionStateCallback(this);
        }

        public void CameraOpened()
        {
            mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
            List<Surface> Targets = new List<Surface>();
            Targets.Add(multiReader.Surface);
            mCameraDevice.CreateCaptureSession(Targets, StateCallback, null);
        }

        public override void Dispose()
        {
            StopStream();
            StopBackgroundThread();
        }

        public void FrameReceived(byte[] bytes, SKImage sKImage)
        {
            if (!Paused)
                Main.Post(delegate
                {
                    FrameReady?.Invoke(this, new FrameReadyEventArgs() { FrameBuffer = bytes, Image = sKImage });
                }, null);
        }

        public override void Load(string FriendlyName, int FrameRate, int Height, int Width, string Encoding)
        {
            try
            {
                if (ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.Camera) != Permission.Granted)
                {
                    MainActivity.Instance.RequestPermissions(new string[1] { Manifest.Permission.Camera }, Main);
                }
                CameraID = FriendlyName;
                CameraManager manager = (CameraManager)MainActivity.Instance.GetSystemService(Context.CameraService);
                CameraCharacteristics characteristics = manager.GetCameraCharacteristics(FriendlyName);
                int SensorRotation = ((Java.Lang.Integer)characteristics.Get(CameraCharacteristics.SensorOrientation)).IntValue();
                OrientationToDegrees.TryGetValue(MainActivity.Instance.WindowManager.DefaultDisplay.Rotation, out int DeviceRotation);
                DegreesToRotateImage = (DeviceRotation + SensorRotation + 270) % 360;
            }
            catch
            {
            }
        }

        public override void PauseStream(bool Toggle)
        {
            Paused = Toggle;
        }

        public override List<string> Scan()
        {
            string[] cameras = manager.GetCameraIdList();
            List<string> extCameras = new List<string>();
            foreach (string x in cameras)
            {
                try
                {
                    CameraCharacteristics charicteristics = manager.GetCameraCharacteristics(x);
                    extCameras.Add(x);
                }
                catch
                {
                }
            }
            return extCameras;
        }

        public override List<VideoType> ScanDevice(string friendlyName)
        {
            List<VideoType> Videotypes = new List<VideoType>();
            Videotypes.Add(new VideoType(friendlyName, 15, 600, 800, "Jpeg"));
            return Videotypes;
        }

        public override void StartStream()
        {
            Task.Run(() =>
            {
                multiReader = ImageReader.NewInstance(800, 600, ImageFormatType.Jpeg, 10);
                bool supported = context.PackageManager.HasSystemFeature(PackageManager.FeatureCameraExternal);
                StartBackgroundThread();
                ImageReaderCallback = new ImageReaderCallback(this);
                multiReader.SetOnImageAvailableListener(ImageReaderCallback, mBackgroundHandler);
                manager.OpenCamera(CameraID, listener, mBackgroundHandler);
            });
        }

        public override void StopStream()
        {
            try
            {
                if (Session != null)
                    Session.Close();
            }
            catch
            {
            }
        }

        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }

        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch
            {
            }
        }
    }
}