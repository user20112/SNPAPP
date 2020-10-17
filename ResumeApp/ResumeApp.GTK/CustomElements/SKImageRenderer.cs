using ResumeApp.CustomElements;
using ResumeApp.GTK.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(SKImage), typeof(SKImageRenderer))]

namespace ResumeApp.GTK.CustomElements
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