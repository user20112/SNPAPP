using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.CustomElements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterListView : ListView
    {
        public BetterListView()
        {
            InitializeComponent();
        }
    }
}