using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(SKImage), typeof(SKImageRenderer))]

namespace ResumeApp.UWP.CustomElements
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
    }
}