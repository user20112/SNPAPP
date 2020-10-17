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
    public partial class BLEOPMPage : BaseContentPage
    {
        public IAdapter adapter;
        private readonly BLEOPMViewModel VM;
        private bool AlreadyEntered = false;
        private List<IDevice> devices;
        private List<IDevice> KnownDevices;
        private OPMPage OPMPage;

        public BLEOPMPage(OPMPage OPMpage) : base()
        {
            InitializeComponent();
            OPMPage = OPMpage;
            VM = BindingContext as BLEOPMViewModel;
            Title = "BLE Opm";
        }

        public void ScanTimeout(object sender, EventArgs e)
        {
            ScanButton.Text = "Start Scan";
            adapter.StopScanningForDevices();
        }

        public void SetParentPage(OPMPage OPMPage)
        {
            this.OPMPage = OPMPage;
            this.adapter = DependencyService.Get<IAdapter>();
        }

        public void StopScanning()
        {
            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
            ScanButton.Text = "Start Scan";
            Task.Run(() =>
            {
                if (adapter.IsScanning)
                {
                    adapter.StopScanningForDevices();
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.devices = new List<IDevice>();
            KnownDevices = new List<IDevice>();
            adapter.DeviceDiscovered += DeviceDiscovered;
            adapter.ScanTimeoutElapsed += ScanTimeout;
            StartScanning();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            adapter.DeviceDiscovered -= DeviceDiscovered;
            adapter.ScanTimeoutElapsed -= ScanTimeout;
            Task.Run(() =>
            {
                if (adapter.IsScanning)
                {
                    adapter.StopScanningForDevices();
                }
            });
        }

        private void DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            if (e.Device.Name != null)
            {
                if (e.Device.Name == "SNP OPM")
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
                    VM.DetectedDevicesList.Add(e.Device.Name + "-" + (VM.DetectedDevicesList.Count + 1).ToString());
                }
            }
        }

        private void ReadingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                StopScanning();
                adapter.ScanTimeoutElapsed -= ScanTimeout;
                OPMPage.SetDevice(adapter, devices[VM.DetectedDevicesList.IndexOf((sender as ListView).SelectedItem as string)]);
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
                if (adapter.IsScanning)
                {
                    adapter.StopScanningForDevices();
                }
                else
                {
                    devices.Clear();
                    adapter.StartScanningForDevices(forService);
                }
            }
            catch
            {
            }
        }
    }
}