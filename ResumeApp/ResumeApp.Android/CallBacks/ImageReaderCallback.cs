using Android.Media;
using Java.Nio;
using SkiaSharp;

namespace ResumeApp.Droid.CallBacks
{
    public class ImageReaderCallback : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private PlatformSpecific.VideoSource Owner;

        public ImageReaderCallback(PlatformSpecific.VideoSource owner)
        {
            Owner = owner;
        }

        public void OnImageAvailable(ImageReader reader)
        {
            Image image = reader.AcquireNextImage();
            ByteBuffer buffer = image.GetPlanes()[0].Buffer;
            byte[] bytes = new byte[buffer.Remaining()];
            buffer.Get(bytes);
            Owner.FrameReceived(bytes, SKImage.FromEncodedData(bytes));
            image.Close();
        }
    }
}