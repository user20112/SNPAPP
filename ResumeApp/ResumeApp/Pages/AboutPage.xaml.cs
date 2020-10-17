using ResumeApp.Classes;
using ResumeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : BaseContentPage
    {
        private readonly AboutViewModel VM;

        public AboutPage()
        {
            InitializeComponent();
            Title = "About";
            VM = BindingContext as AboutViewModel; VM.AboutListSource.Add(MainPage.HomePageItems[0]);//serial Page
            VM.AboutListSource.Add(MainPage.HomePageItems[1]);//usbpage
            //VM.AboutListSource.Add(MainPage.HomePageItems[2]);//wifipage
            VM.AboutListSource.Add(MainPage.HomePageItems[3]);//ble page
            VM.AboutListSource.Add(MainPage.HomePageItems[4]);//opm page
            //VM.AboutListSource.Add(MainPage.HomePageItems[5]);//analysis page
            VM.AboutListSource.Add(MainPage.HomePageItems[6]);//video page
            VM.AboutListSource.Add(MainPage.HomePageItems[7]);//snake page
            //VM.AboutListSource.Add(MainPage.HomePageItems[8]);//schedule page
            //VM.AboutListSource.Add(MainPage.HomePageItems[9]);//sqllite page
            //VM.AboutListSource.Add(MainPage.HomePageItems[10]);//SNPMonitorPage
            VM.AboutListSource.Add(MainPage.HomePageItems[11]);//MovingWeb
            VM.AboutListSource.Add(MainPage.HomePageItems[12]);//ActiveMQ
            //VM.AboutListSource.Add(MainPage.HomePageItems[13]);//LocalChat
            //VM.AboutListSource.Add(MainPage.HomePsudo apt install apache2ageItems[14]);//GPS/Location
            foreach (HomePageItem item in MainPage.HomePageItems)
                VM.AboutListSource.Add(item);
        }

        private void AboutList_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem == null)
                return;
            ((sender as ListView).SelectedItem as HomePageItem).Expanded = !((sender as ListView).SelectedItem as HomePageItem).Expanded;
            RefreshListView();
        }

        private void RefreshListView()
        {
            AboutList.ItemsSource = null;
            AboutList.SelectedItem = null;
            AboutList.ItemsSource = VM.AboutListSource;
            VM.RefreshListView();
        }

        private void WebsiteTapped(object sender, System.EventArgs e)
        {
        }

        private void EmailTapped(object sender, System.EventArgs e)
        {
        }

        private void PhoneTapped(object sender, System.EventArgs e)
        {
        }
    }
}