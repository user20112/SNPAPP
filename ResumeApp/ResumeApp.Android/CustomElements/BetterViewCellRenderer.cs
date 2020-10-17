using Android.Content;
using Android.Views;
using ResumeApp.CustomElements;
using ResumeApp.Droid.CustomElements;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BetterViewCell), typeof(BetterViewCellRenderer))]

namespace ResumeApp.Droid.CustomElements
{
    public class BetterViewCellRenderer : ViewCellRenderer
    {
        private Android.Views.View _cellCore;
        private bool _selected;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _cellCore = base.GetCellCore(item, convertView, parent, context);
            _selected = false;
            _cellCore.SetBackgroundColor(_cellCore.Selected ? (Cell as BetterViewCell).SelectedBackgroundColor.ToAndroid() : (Cell as BetterViewCell).DefualtBackgroundColor.ToAndroid());
            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);
            if (args.PropertyName == "IsSelected")
            {
                _selected = !_selected;
                if (_selected)
                {
                    BetterViewCell extendedViewCell = sender as BetterViewCell;
                    _cellCore.SetBackgroundColor(extendedViewCell.SelectedBackgroundColor.ToAndroid());
                }
                else
                {
                    BetterViewCell extendedViewCell = sender as BetterViewCell;
                    _cellCore.SetBackgroundColor(extendedViewCell.DefualtBackgroundColor.ToAndroid());
                }
            }
        }
    }
}