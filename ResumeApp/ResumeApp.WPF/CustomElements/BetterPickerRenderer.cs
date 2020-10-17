using ResumeApp.CustomElements;
using ResumeApp.WPF.CustomElements;
using System.ComponentModel;
using System.Windows.Media;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(BetterPicker), typeof(BetterPickerRenderer))]

namespace ResumeApp.WPF.CustomElements
{
    public class BetterPickerRenderer : PickerRenderer
    {
        public BetterPickerRenderer() : base()
        {
        }

        public static void Init()
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "PopupBackground")
            {
                if ((Element as BetterPicker).PopupBackground != null)
                {
                    Control.Background = new SolidColorBrush((Element as BetterPicker).PopupBackground.ToColor());
                }
            }
            if (e.PropertyName == "PopupTextColor")
            {
                if ((Element as BetterPicker).PopupTextColor != null)
                {
                    Control.Foreground = new SolidColorBrush((Element as BetterPicker).PopupTextColor.ToColor());
                }
            }
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