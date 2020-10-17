using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.WPF.CustomElements
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