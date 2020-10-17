using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterViewCell : ViewCell
    {
        public BetterViewCell()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty DefualtBackgroundColorProperty =
           BindableProperty.Create("DefualtBackgroundColor",
                                   typeof(Color),
                                   typeof(BetterViewCell),
                                   Color.Default);

        public static readonly BindableProperty SelectedBackgroundColorProperty =
                    BindableProperty.Create("SelectedBackgroundColor",
                                    typeof(Color),
                                    typeof(BetterViewCell),
                                    Color.Default);

        public Color DefualtBackgroundColor
        {
            get { return (Color)GetValue(DefualtBackgroundColorProperty); }
            set { SetValue(DefualtBackgroundColorProperty, value); }
        }

        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }
    }
}