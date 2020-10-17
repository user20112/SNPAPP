using ResumeApp.Classes;
using ResumeApp.Classes.Adapters;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using ResumeApp.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLEPage : BaseContentPage
    {
        private BLEViewModel VM;
        private bool DeviceSet = false;

        public BLEPage()
        {
            InitializeComponent();
            Title = "BLE";
            VM = this.BindingContext as BLEViewModel;
        }

        public override void OnFirstAppearing()
        {
            base.OnFirstAppearing();
            NavigateTo(new BLEPairPage(this));
        }

        public override void OnReAppearing()
        {
            base.OnReAppearing();
            if (!DeviceSet)
                NavigateBack();
        }

        private IDevice SelectedDevice;

        internal void SetDevice(IDevice device)
        {
            DeviceSet = true;
            VM.ServicesListSource.Clear();
            if (SelectedDevice != null)
            {
                Adapter.DisconnectDevice(SelectedDevice);
            }
            SelectedDevice = device;
            Adapter.DeviceConnected += ConnectedToDevice;
            Adapter.ConnectToDevice(SelectedDevice);
        }

        private void ConnectedToDevice(object sender, BluetoothDeviceConnectionEventArgs e)
        {
            e.Device.ServicesDiscovered += ServicesDiscovered;
            Main.Send(delegate
            {
                DisplayLabel.Text = "Discovering Services";
            }, null);
            e.Device.DiscoverServices();
        }

        private void ServicesDiscovered(object sender, EventArgs e)
        {
            Main.Send(delegate
            {
                DisplayLabel.Text = "Services Discovered";
            }, null);
            for (int x = 0; x < SelectedDevice.Services.Count; x++)
            {
                VM.ServicesListSource.Add(new ServiceListViewAdapter(SelectedDevice.Services[x]));
            }
        }

        public override void BackButtonPressed()
        {
            base.BackButtonPressed();
        }
    }
}