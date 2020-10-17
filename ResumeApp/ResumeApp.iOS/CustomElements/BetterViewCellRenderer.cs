using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BetterViewCell), typeof(BetterViewCellRenderer))]

namespace ResumeApp.iOS.CustomElements
{
    public class BetterViewCellRenderer : ViewCellRenderer
    {
        public BetterViewCellRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            UITableViewCell cell = base.GetCell(item, reusableCell, tv);
            BetterViewCell view = item as BetterViewCell;
            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = view.SelectedBackgroundColor.ToUIColor(),
            };
            return cell;
        }
    }
}