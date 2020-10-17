using ResumeApp.CustomElements;
using ResumeApp.UWP.CustomElements;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BetterPicker), typeof(BetterPickerRenderer))]

namespace ResumeApp.UWP.CustomElements
{
    public class BetterPickerRenderer : PickerRenderer
    {
        public BetterPickerRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
            }
        }
    }
}