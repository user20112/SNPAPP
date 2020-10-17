using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using ResumeApp.CustomElements;
using ResumeApp.Droid.CustomElements;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BetterPicker), typeof(BetterPickerRenderer))]

namespace ResumeApp.Droid.CustomElements
{
    public class BetterPickerRenderer : PickerRenderer
    {
        IElementController ElementController => Element as IElementController;
        private AlertDialog _dialog;

        public BetterPickerRenderer() : base(MainActivity.Instance)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "PopupBackground")
            {
                if ((Element as BetterPicker).PopupBackground != null)
                {
                    //Control.Image = (Element as BetterPicker).PopupBackground;
                }
            }
            if (e.PropertyName == "PopupTextColor")
            {
                if ((Element as BetterPicker).PopupTextColor != null)
                {
                    //Control.Image = (Element as BetterPicker).PopupTextColor;
                }
            }
        }

        public static void Init()
        {
        }

        private Picker picker;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
            if (Control != null)
                Control.Click += Control_Click;
        }

        protected override void Dispose(bool disposing)
        {
            Control.Click -= Control_Click;
            base.Dispose(disposing);
        }

        private void SetPickerDividerColor(CustomNumberPicker picker, Android.Graphics.Color color)
        {
            Java.Lang.Reflect.Field[] fields = picker.Class.GetDeclaredFields();
            foreach (Java.Lang.Reflect.Field pf in fields)
            {
                if (pf.Name.Equals("mSelectionDivider"))
                {
                    pf.Accessible = true;
                    // set the color as transparent
                    pf.Set(picker, new ColorDrawable(color));
                }
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            Picker model = Element;
            CustomNumberPicker picker = new CustomNumberPicker(Context);
            picker.TextColor = (Element as BetterPicker).PopupTextColor.ToAndroid();
            picker.ValueChanged += SelectionChanged;
            if (model.Items != null && model.Items.Any())
            {
                picker.MaxValue = model.Items.Count - 1;
                picker.MinValue = 0;
                picker.SetBackgroundColor((Element as BetterPicker).PopupBackground.ToAndroid());
                picker.SetDisplayedValues(model.Items.ToArray());
                picker.TextColor = (Element as BetterPicker).PopupTextColor.ToAndroid();
                SetPickerDividerColor(picker, (Element as BetterPicker).PopupTextColor.ToAndroid());
                picker.WrapSelectorWheel = false;
                picker.Value = model.SelectedIndex;
            }
            LinearLayout layout = new LinearLayout(Context) { Orientation = Orientation.Vertical };
            layout.SetBackgroundColor((Element as BetterPicker).PopupBackground.ToAndroid());
            layout.AddView(picker);
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, true);
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetView(layout);
            builder.SetTitle(model.Title ?? "");
            _dialog = builder.Create();
            Android.Widget.Button Button = _dialog.GetButton((int)DialogButtonType.Positive);
            if (Button != null)
                Button.SetTextColor((Element as BetterPicker).PopupTextColor.ToAndroid());
            Button = _dialog.GetButton((int)DialogButtonType.Negative);
            if (Button != null)
                Button.SetTextColor((Element as BetterPicker).PopupTextColor.ToAndroid());
            Button = _dialog.GetButton((int)DialogButtonType.Neutral);
            if (Button != null)
                Button.SetTextColor((Element as BetterPicker).PopupTextColor.ToAndroid());
            _dialog.Window.SetBackgroundDrawable(new ColorDrawable((Element as BetterPicker).PopupBackground.ToAndroid()));
            _dialog.DismissEvent += (ssender, args) =>
            {
                ElementController?.SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
            };
            _dialog.Show();
        }

        private void SelectionChanged(object sender, NumberPicker.ValueChangeEventArgs e)
        {
            Element.SelectedIndex = e.NewVal;
        }
    }
}