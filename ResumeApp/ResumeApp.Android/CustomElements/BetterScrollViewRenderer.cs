using ResumeApp.CustomElements;
using ResumeApp.Droid.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.Droid.CustomElements
{
    public class BetterScrollViewRenderer : ScrollViewRenderer
    {
        public BetterScrollViewRenderer() : base(MainActivity.Instance)
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }
    }
}