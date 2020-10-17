using ResumeApp.Classes;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmguPage : BaseContentPage
    {
        public EmguPage()
        {
            InitializeComponent();
            Title = "Emgu";
        }
    }
}