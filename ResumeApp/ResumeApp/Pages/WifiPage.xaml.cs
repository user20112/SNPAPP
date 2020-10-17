using ResumeApp.Classes;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiPage : BaseContentPage
    {
        public WifiPage()
        {
            InitializeComponent();
            Title = "Wifi";
        }
    }
}