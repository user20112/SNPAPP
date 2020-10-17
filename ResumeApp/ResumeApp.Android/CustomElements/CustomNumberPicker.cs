using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace ResumeApp.Droid.CustomElements
{
    public class CustomNumberPicker : NumberPicker
    {
        public CustomNumberPicker(Context context) : base(context)
        {
        }

        private Color _TextColor = Color.White;

        public Color TextColor
        {
            get
            {
                return _TextColor;
            }
            set
            {
                _TextColor = value;
                this.Invalidate();
            }
        }

        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {
            base.AddView(child, index, @params);
            UpdateView(child);
        }

        public void UpdateView(View view)
        {
            if (view is EditText EditView)
            {
                EditView.SetTextColor(_TextColor);
                EditView.Enabled = false;
            }
        }
    }
}