using Android.Hardware.Camera2;
using ResumeApp.Droid.PlatformSpecific;

namespace ResumeApp.Droid.CallBacks
{
    public class CameraCaptureSessionStateCallback : CameraCaptureSession.StateCallback
    {
        private readonly VideoSource Owner;

        public CameraCaptureSessionStateCallback(VideoSource owner)
        {
            Owner = owner;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (null == Owner.mCameraDevice)
            {
                return;
            }
            // When the session is ready, we start displaying the preview.
            Owner.Session = session;
            try
            {
                // Auto focus should be continuous for camera preview.
                Owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                Owner.mPreviewRequestBuilder.Set(CaptureRequest.JpegOrientation, new Java.Lang.Integer(Owner.DegreesToRotateImage));
                // Finally, we start displaying the camera preview.
                Owner.mPreviewRequestBuilder.AddTarget(Owner.multiReader.Surface);
                Owner.mPreviewRequest = Owner.mPreviewRequestBuilder.Build();
                Owner.Session.Capture(Owner.mPreviewRequestBuilder.Build(), Owner.callback, Owner.mBackgroundHandler);
                //Owner.Session.SetRepeatingRequest(Owner.mPreviewRequestBuilder.Build(), Owner.callback, Owner.mBackgroundHandler);//  not working not sure why
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
        }
    }
}