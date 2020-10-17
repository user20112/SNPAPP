using ResumeApp.CustomElements;
using ResumeApp.GTK.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK.Cells;

[assembly: ExportRenderer(typeof(BetterViewCell), typeof(BetterViewCellRenderer))]

namespace ResumeApp.GTK.CustomElements
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