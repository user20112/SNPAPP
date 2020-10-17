using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BetterViewCell), typeof(BetterViewCellRenderer))]

namespace ResumeApp.UWP.CustomElements
{
    public class BetterViewCellRenderer : ViewCellRenderer
    {
        public BetterViewCellRenderer() : base()
        {
        }

        public static void Init()
        {
        }
    }
}