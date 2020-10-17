using Android.Hardware.Camera2;
using ResumeApp.Droid.PlatformSpecific;

namespace ResumeApp.Droid.CallBacks
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private VideoSource Owner;

        public CameraCaptureSessionCallback(VideoSource owner)
        {
            Owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            Process(result);
        }

        public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
        {
            Process(partialResult);
        }

        private void Process(CaptureResult result)
        {
            try
            {
                Owner.Session.Capture(Owner.mPreviewRequestBuilder.Build(), Owner.callback, Owner.mBackgroundHandler);
            }
            catch
            {// when stopping the stream this will error 1 time.
            }
        }
    }
}