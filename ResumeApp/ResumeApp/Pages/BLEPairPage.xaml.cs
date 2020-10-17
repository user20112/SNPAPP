using ResumeApp.Classes;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using ResumeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLEPairPage : BaseContentPage
    {
        private readonly BLEPairViewModel VM;
        private bool AlreadyEntered = false;
        private List<IDevice> devices;
        private List<IDevice> KnownDevices;
        private BLEPage BLEPage;

        public BLEPairPage(BLEPage OPMpage) : base()
        {
            InitializeComponent();
            BLEPage = OPMpage;
            VM = BindingContext as BLEPairViewModel;
            Title = "BLE Searching";
        }

        public void ScanTimeout(object sender, EventArgs e)
        {
            ScanButton.Text = "Start Scan";
            Adapter.StopScanningForDevices();
        }

        public void StopScanning()
        {
            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
            ScanButton.Text = "Start Scan";
            Task.Run(() =>
            {
                if (Adapter.IsScanning)
                {
                    Adapter.StopScanningForDevices();
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.devices = new List<IDevice>();
            KnownDevices = new List<IDevice>();
            Adapter.DeviceDiscovered += DeviceDiscovered;
            Adapter.ScanTimeoutElapsed += ScanTimeout;
            StartScanning();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Adapter.DeviceDiscovered -= DeviceDiscovered;
            Adapter.ScanTimeoutElapsed -= ScanTimeout;
            Task.Run(() =>
            {
                if (Adapter.IsScanning)
                {
                    Adapter.StopScanningForDevices();
                }
            });
        }

        private void DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            bool NewDevice = true;
            foreach (IDevice dev in KnownDevices)
            {
                if (dev.ID == e.Device.ID)
                {
                    NewDevice = false;
                    break;
                }
            }
            if (NewDevice)
            {
                KnownDevices.Add(e.Device);
            }
            devices.Add(e.Device);
            VM.DetectedDevicesList.Add(e.Device.ID + "-" + (VM.DetectedDevicesList.Count + 1).ToString());
        }

        private void ReadingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                StopScanning();
                Adapter.ScanTimeoutElapsed -= ScanTimeout;
                BLEPage.SetDevice(devices[VM.DetectedDevicesList.IndexOf((sender as ListView).SelectedItem as string)]);
                NavigateBack();
                (sender as ListView).SelectedItem = null;
            }
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            if (ScanButton.Text == "Start Scan")
                StartScanning();
            else
                StopScanning();
        }

        private void StartScanning()
        {
            BusyIndicator.IsRunning = true;
            BusyIndicator.IsVisible = true;
            VM.DetectedDevicesList.Clear();
            devices.Clear();
            KnownDevices.Clear();
            if (PlatformSpecificInterface.IsLocationOn())
            {
                if (PlatformSpecificInterface.IsBluetoothOn())
                {
                    StartScanning(Guid.Parse("00035b03-58e6-07dd-021a-08123a000300"));
                    ScanButton.Text = "Stop Scan";
                }
                else
                {
                    PlatformSpecificInterface.RequestBluetoothPermission();
                    AlreadyEntered = true;
                    if (!AlreadyEntered)
                        StartScanning();
                    else
                    {
                        ShowToast("Bluetooth Permission Required to scan.");
                        StopScanning();
                    }
                }
            }
            else
            {
                if (!AlreadyEntered)
                {
                    AlreadyEntered = true;
                    PlatformSpecificInterface.RequestLocationPermission();
                    StartScanning();
                }
                else
                {
                    ShowToast("Location Permission Required to scan. Please check that you have locations enabled aswell.");
                    StopScanning();
                }
            }
        }

        private void StartScanning(Guid forService)
        {
            try
            {
                if (Adapter.IsScanning)
                {
                    Adapter.StopScanningForDevices();
                }
                else
                {
                    devices.Clear();
                    Adapter.StartScanningForDevices(forService);
                }
            }
            catch
            {
            }
        }
    }
}