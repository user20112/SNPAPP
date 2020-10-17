using ResumeApp.WPF.Classes;
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace ResumeApp.WPF
{
    public partial class MainWindow : FormsApplicationPage
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            Forms.Init();
            LoadApplication(new ResumeApp.App());
        }

        private void OnLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowAspectRatio.Register((Window)this);
        }
    }
}