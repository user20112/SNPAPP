using Android.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;
using NativeFeatures.Droid;
using SkiaSharp;
using SkiaSharp.Views.Android;

[assembly: Xamarin.Forms.Dependency(typeof(Convert))]

namespace NativeFeatures.Droid
{
    public class Convert
    {
        public static SKColor ColorToSKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKImage ImageToSKImage(Image<Bgr, byte> image)
        {
            using Bitmap temp = image.ToBitmap();
            return temp.ToSKImage();
        }

        public static SKImage ImageToSKImage(Image<Gray, byte> image)
        {
            using Bitmap temp = image.ToBitmap();
            return temp.ToSKImage();
        }

        public static Color SKColorToColor(SKColor color)
        {
            return new Color(color.Red, color.Green, color.Blue, color.Alpha);
        }

        public static Image<Gray, byte> SKImageToGrayImage(SKImage image)
        {
            using Bitmap temp = AndroidExtensions.ToBitmap(SKBitmap.FromImage(image));
            return new Image<Gray, byte>(temp);
        }

        public static Image<Bgr, byte> SKImageToImage(SKImage image)
        {
            using Bitmap temp = AndroidExtensions.ToBitmap(SKBitmap.FromImage(image));
            return new Image<Bgr, byte>(temp);
        }
    }
}