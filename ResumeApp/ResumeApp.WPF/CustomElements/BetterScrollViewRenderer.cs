using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using System.Windows.Controls;
using System.Windows.Input;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(BetterScrollView), typeof(BetterScrollViewRenderer))]

namespace ResumeApp.WPF.CustomElements
{
    public class BetterScrollViewRenderer : ScrollViewRenderer
    {
        public BetterScrollViewRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ScrollView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                Control.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
            if (e.NewElement != null)
            {
                Control.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                Control.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                Control.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (((BetterScrollView)Element).ScrollHorizontally)
            {
                if (args.Delta < 0)
                    Control.LineRight();
                else
                    Control.LineLeft();
                args.Handled = true;
            }
        }
    }
}