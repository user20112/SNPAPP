using Android.Hardware.Camera2;
using ResumeApp.Droid.PlatformSpecific;

namespace ResumeApp.Droid.CallBacks
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly VideoSource Owner;

        public CameraStateListener(VideoSource owner)
        {
            Owner = owner;
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            cameraDevice.Close();
            Owner.mCameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            cameraDevice.Close();
            Owner.mCameraDevice = null;
            if (Owner == null)
                return;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            Owner.mCameraDevice = cameraDevice;
            Owner.CameraOpened();
        }
    }
}