using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.UWP.CustomElements
{
    public class BetterListViewRenderer : ListViewRenderer
    {
        public BetterListViewRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
        }
    }
}