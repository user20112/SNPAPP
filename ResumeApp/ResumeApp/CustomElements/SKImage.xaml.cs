using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SKImage : Image
    {
        public SKImage()
        {
            InitializeComponent();
        }

        public static BindableProperty SKImageSourceProperty = BindableProperty.Create(
  propertyName: "SKImageSource",
  returnType: typeof(SkiaSharp.SKImage),
  declaringType: typeof(SkiaSharp.SKImage),
  defaultValue: null,
  defaultBindingMode: BindingMode.OneWay,
  propertyChanged: HandleTextPropertyChanged);

        public SkiaSharp.SKImage SKImageSource
        {
            get
            {
                return (SkiaSharp.SKImage)base.GetValue(SKImageSourceProperty);
            }
            set
            {
                if (SKImageSource != value)
                    base.SetValue(SKImageSourceProperty, value);
            }
        }

        public void UpdateImage(SkiaSharp.SKImage image)
        {
            SKImageSource = image;
            OnPropertyChanged("SKImageSource");
        }

        public void UpdateImage(byte[] image)
        {
            SKImageSource = SkiaSharp.SKImage.FromEncodedData(image);
            OnPropertyChanged("SKImageSource");
        }

        public void UpdateImage()
        {
            OnPropertyChanged("SKImageSource");
        }

        private static void HandleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SKImage targetView;
            targetView = (SKImage)bindable;
            if (targetView != null)
                targetView.UpdateImage();
        }
    }
}