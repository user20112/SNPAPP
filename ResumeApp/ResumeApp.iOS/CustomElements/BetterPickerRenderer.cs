using ResumeApp.CustomElements;
using ResumeApp.iOS.CustomElements;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BetterPicker), typeof(BetterPickerRenderer))]

namespace ResumeApp.iOS.CustomElements
{
    public class BetterPickerRenderer : PickerRenderer
    {
        public BetterPickerRenderer() : base()
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "PopupBackground")
            {
                if ((Element as BetterPicker).PopupBackground != null)
                {
                    PickerView.BackgroundColor = (Element as BetterPicker).PopupBackground.ToUIColor();
                }
            }
            if (e.PropertyName == "PopupTextColor")
            {
                if ((Element as BetterPicker).PopupTextColor != null)
                {
                    PickerView.TintColor = (Element as BetterPicker).PopupTextColor.ToUIColor();
                    Control.TextColor = (Element as BetterPicker).PopupTextColor.ToUIColor();
                }
            }
        }

        private UIPickerView PickerView;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                PickerView = (UIPickerView)Control.InputView;
            }
        }
    }
}