using ResumeApp.CustomElements;
using ResumeApp.GTK.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.GTK.CustomElements
{
    public class BetterScrollViewRenderer : ScrollViewRenderer
    {
        public BetterScrollViewRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ScrollView> e)
        {
            base.OnElementChanged(e);
        }
    }
}