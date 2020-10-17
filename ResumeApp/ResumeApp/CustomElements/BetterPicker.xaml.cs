using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterPicker : Picker
    {
        public static BindableProperty PopupBackgroundProperty = BindableProperty.Create(
  propertyName: "PopupBackground",
  returnType: typeof(Color),
  declaringType: typeof(Color),
  defaultValue: Color.Gray,
  defaultBindingMode: BindingMode.OneWay,
  propertyChanged: HandleBackgroundChangedEvent);

        private static void HandleBackgroundChangedEvent(BindableObject bindable, object oldValue, object newValue)
        {
            BetterPicker targetView = (BetterPicker)bindable;
            if (targetView != null)
                targetView.OnPropertyChanged("PopupBackground");
        }

        public static BindableProperty PopupTextProperty = BindableProperty.Create(
  propertyName: "PopupTextColor",
  returnType: typeof(Color),
  declaringType: typeof(Color),
  defaultValue: Color.White,
  defaultBindingMode: BindingMode.OneWay,
  propertyChanged: HandleTextChangedEvent);

        private static void HandleTextChangedEvent(BindableObject bindable, object oldValue, object newValue)
        {
            BetterPicker targetView = (BetterPicker)bindable;
            if (targetView != null)
                targetView.OnPropertyChanged("PopupTextColor");
        }

        public Color PopupBackground
        {
            get
            {
                return (Color)base.GetValue(PopupBackgroundProperty);
            }
            set
            {
                if (PopupBackground != value)
                    base.SetValue(PopupBackgroundProperty, value);
            }
        }

        public Color PopupTextColor
        {
            get
            {
                return (Color)base.GetValue(PopupTextProperty);
            }
            set
            {
                if (PopupTextColor != value)
                    base.SetValue(PopupTextProperty, value);
            }
        }

        public BetterPicker()
        {
            InitializeComponent();
        }
    }
}