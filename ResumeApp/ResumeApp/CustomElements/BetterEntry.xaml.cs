using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterEntry : Entry
    {
        public BetterEntry()
        {
            InitializeComponent();
        }
    }
}