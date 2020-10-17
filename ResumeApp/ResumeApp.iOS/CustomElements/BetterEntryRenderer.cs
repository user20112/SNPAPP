using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BetterEntry), typeof(BetterEntryRenderer))]

namespace ResumeApp.iOS.CustomElements
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