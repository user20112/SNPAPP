using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterScrollView : ScrollView
    {
        public bool ScrollHorizontally { get; set; } = false;

        public BetterScrollView()
        {
            InitializeComponent();
        }
    }
}