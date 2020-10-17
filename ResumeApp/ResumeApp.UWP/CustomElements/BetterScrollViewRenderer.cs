using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.UWP.CustomElements
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