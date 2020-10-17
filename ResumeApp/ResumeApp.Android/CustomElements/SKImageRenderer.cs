using Android.Graphics;
using ResumeApp.Droid.CustomElements;
using SkiaSharp;
using SkiaSharp.Views.Android;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Image = Xamarin.Forms.Image;

[assembly: ExportRenderer(typeof(ResumeApp.CustomElements.SKImage), typeof(SKImageRenderer))]

namespace ResumeApp.Droid.CustomElements
{
    public class SKImageRenderer : ImageRenderer
    {
        private int counter = 0;
        private Bitmap ThisImage;

        public SKImageRenderer() : base(Android.App.Application.Context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                //element is disapearing
            }
            if (e.NewElement != null)
            {
                //element is being created
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "SKImageSource")
            {
                SkiaSharp.SKImage Temp = (Element as ResumeApp.CustomElements.SKImage).SKImageSource;
                if (Temp != null)
                {
                    if (counter++ == 10)
                    {
                        Task.Run(() =>
                        {
                            GC.Collect();
                            counter = 0;
                        });
                    }
                    ThisImage = AndroidExtensions.ToBitmap(SKBitmap.FromImage(Temp));
                    if (ThisImage != null)
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Control.SetImageBitmap(ThisImage);
                            Control.Invalidate();
                        });
                    else
                    {
                    }
                }
            }
        }
    }
}