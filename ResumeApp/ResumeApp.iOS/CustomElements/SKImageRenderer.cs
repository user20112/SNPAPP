using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using SkiaSharp.Views.iOS;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SKImage), typeof(SKImageRenderer))]

namespace ResumeApp.iOS.CustomElements
{
    public class SKImageRenderer : ImageRenderer
    {
        public SKImageRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "SKImageSource")
            {
                if ((Element as SKImage).SKImageSource != null)
                {
                    Control.Image = (Element as SKImage).SKImageSource.ToUIImage();
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
        }
    }
}