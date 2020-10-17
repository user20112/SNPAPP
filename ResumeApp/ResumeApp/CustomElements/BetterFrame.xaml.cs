using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterFrame : Frame
    {
        private int _Thickness = 0;

        public static BindableProperty BorderColorProperty = BindableProperty.Create(
  propertyName: "BorderColor",
  returnType: typeof(Color),
  declaringType: typeof(Color),
  defaultValue: null,
  defaultBindingMode: BindingMode.OneWay,
  propertyChanged: HandleTracePropertyChanged);

        private static void HandleTracePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            BetterFrame targetView = (BetterFrame)bindable;
            if (targetView != null)
                targetView.BorderColor = (Color)newValue;
        }

        public int Thickness
        {
            get { return _Thickness; }
            set
            {
                Padding = new Thickness(value); _Thickness = value;
            }
        }

        public float Corner
        {
            get { return InnerFrame.CornerRadius; }
            set
            {
                InnerFrame.CornerRadius = value; OuterFrame.CornerRadius = value;
            }
        }

        public new View Content { get { return InnerFrame.Content; } set { InnerFrame.Content = value; } }
        public Color InnerColor { get { return InnerFrame.BackgroundColor; } set { InnerFrame.BackgroundColor = value; } }
        public Color OuterColor { get { return OuterFrame.BackgroundColor; } set { OuterFrame.BackgroundColor = value; } }

        public new Color BorderColor
        {
            get { return InnerFrame.BorderColor; }
            set
            {
                InnerFrame.BorderColor = value; OuterFrame.BorderColor = value;
            }
        }

        public BetterFrame()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            for (Element Parent = this.Parent; Parent != null; Parent = Parent.Parent)
            {
                try
                {
                    Color background = Parent.GetPropertyIfSet<Color>(BackgroundColorProperty, Color.Transparent);
                    if (background != Color.Transparent && InnerFrame.BackgroundColor != Color.Transparent)
                    {
                        InnerFrame.BackgroundColor = background;
                        break;
                    }
                }
                catch
                {
                }
            }
        }
    }
}