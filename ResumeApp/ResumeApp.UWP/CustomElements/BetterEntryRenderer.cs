using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BetterEntry), typeof(BetterEntryRenderer))]

namespace ResumeApp.UWP.CustomElements
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