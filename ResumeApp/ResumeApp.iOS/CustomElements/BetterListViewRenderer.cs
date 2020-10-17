using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BetterListView), typeof(BetterListViewRenderer))]

namespace ResumeApp.iOS.CustomElements
{
    public class BetterListViewRenderer : ListViewRenderer
    {
        public BetterListViewRenderer() : base()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            InsertRowsAnimation = UITableViewRowAnimation.None;
            DeleteRowsAnimation = UITableViewRowAnimation.None;
            ReloadRowsAnimation = UITableViewRowAnimation.None;
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                InsertRowsAnimation = UITableViewRowAnimation.None;
                DeleteRowsAnimation = UITableViewRowAnimation.None;
                ReloadRowsAnimation = UITableViewRowAnimation.None;
            }
            if (e.NewElement != null)
            {
            }
        }
    }
}