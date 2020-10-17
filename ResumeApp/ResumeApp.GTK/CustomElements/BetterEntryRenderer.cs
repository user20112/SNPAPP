using ResumeApp.CustomElements;
using ResumeApp.GTK.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(BetterEntry), typeof(BetterEntryRenderer))]

namespace ResumeApp.GTK.CustomElements
{
    public class BetterEntryRenderer : EntryRenderer
    {
        public BetterEntryRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);
        }
    }
}