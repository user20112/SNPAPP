using ResumeApp.CustomElements;
using ResumeApp.GTK.CustomElements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(BetterPicker), typeof(BetterPickerRenderer))]

namespace ResumeApp.GTK.CustomElements
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