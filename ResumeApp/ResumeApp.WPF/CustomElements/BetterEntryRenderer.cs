using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(BetterEntry), typeof(BetterEntryRenderer))]

namespace ResumeApp.WPF.CustomElements
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