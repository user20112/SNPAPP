using Emgu.CV;
using Emgu.CV.Structure;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using UIKit;

[assembly: Xamarin.Forms.DependencyAttribute(typeof(NativeFeatures.iOS.Convert))]

namespace NativeFeatures.iOS
{
    public class Convert
    {
        public static SKImage ImageToSKImage(Image<Bgr, byte> image)
        {
            return image.ToUIImage().ToSKImage();
        }

        public static SKImage ImageToSKImage(Image<Gray, byte> image)
        {
            return image.ToUIImage().ToSKImage();
        }

        public static UIColor SKColorToUIColor(SKColor color)
        {
            return new UIColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        public static Image<Gray, byte> SKImageToGrayImage(SKImage image)
        {
            return new Image<Gray, byte>(image.ToUIImage());
        }

        public static Image<Bgr, byte> SKImageToImage(SKImage image)
        {
            return new Image<Bgr, byte>(image.ToUIImage());
        }

        public static SKColor UIColorToSKColor(UIColor color)
        {
            color.GetRGBA(out nfloat r, out nfloat g, out nfloat b, out nfloat a);
            return new SKColor((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}