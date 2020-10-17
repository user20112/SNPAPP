using ResumeApp.Classes;
using ResumeApp.ViewModels;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class USBSelectPage : BaseContentPage
    {
        private USBPage ParentPage;
        private UsbSelectViewModel VM;

        public USBSelectPage(USBPage parentPage)
        {
            InitializeComponent();
            ParentPage = parentPage;
            VM = BindingContext as UsbSelectViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (string x in HIDDeviceInterface.Scan())
                VM.USBDevices.Add(x);
        }

        private void USBList_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ParentPage.SetDevice(e.SelectedItem as string);
            NavigateBack();
        }

        private void ScanButton_Clicked(object sender, System.EventArgs e)
        {
            VM.USBDevices.Clear();
            foreach (string x in HIDDeviceInterface.Scan())
                VM.USBDevices.Add(x);
        }
    }
}