using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(BetterViewCell), typeof(BetterViewCellRenderer))]

namespace ResumeApp.WPF.CustomElements
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