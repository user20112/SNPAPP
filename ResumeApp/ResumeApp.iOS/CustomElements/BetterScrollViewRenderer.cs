using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.iOS.CustomElements
{
    public class BetterScrollViewRenderer : ScrollViewRenderer
    {
        public BetterScrollViewRenderer() : base()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }
    }
}