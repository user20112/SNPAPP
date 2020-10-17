using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using SkiaSharp.Views.Desktop;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(SKImage), typeof(SKImageRenderer))]

namespace ResumeApp.WPF.CustomElements
{
    public class SKImageRenderer : ImageRenderer
    {
        public SKImageRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "SKImageSource")
            {
                if ((Element as SKImage).SKImageSource != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //File.WriteAllBytes("C:\\Users\\devlin\\Desktop\\WiiSaves\\File", (Element as SKImage).SKImageSource.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
                        Control.Source = BitmapToImageSource((Element as SKImage).SKImageSource.ToBitmap());
                    }));
                }
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}