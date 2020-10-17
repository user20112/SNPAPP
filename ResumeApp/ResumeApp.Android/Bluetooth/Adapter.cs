using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using Android.Runtime;
using NativeFeatures.Droid;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(
          typeof(Adapter))]

namespace NativeFeatures.Droid
{
    public class Adapter : Java.Lang.Object, BluetoothAdapter.ILeScanCallback, IAdapter
    {
        public int disconnectCount = 0;
        protected BluetoothAdapter blAdapter;
        protected BluetoothManager blManager;
        protected IList<IDevice> connectedDevices = new List<IDevice>();
        protected IList<IDevice> discoveredDevices = new List<IDevice>();
        protected GattCallback gattCallback;
        protected bool isScanning;
        private BluetoothGatt Bluetoothgatt;
        private Adapter2 NewAdapter;

        public Adapter()
        {
            var appContext = Android.App.Application.Context;
            blManager = (BluetoothManager)appContext.GetSystemService("bluetooth");
            blAdapter = blManager.Adapter;
        }

        void IDisposable.Dispose()
        {
            foreach (IDevice device in ConnectedDevices)
            {
                DisconnectDevice(device);
                device.Dispose();
            }
        }

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceConnected = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceDisconnected = delegate { };

        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };

        public event EventHandler ScanTimeoutElapsed = delegate { };

        public BluetoothGatt Bluetoothgatt1 { get => Bluetoothgatt; set => Bluetoothgatt = value; }

        public IList<IDevice> ConnectedDevices
        {
            get { return connectedDevices; }
        }

        public IList<IDevice> DiscoveredDevices
        {
            get
            {
                return discoveredDevices;
            }
        }

        public bool IsScanning
        {
            get { return isScanning; }
        }

        public object DiagnosticOut { get; private set; }

        public void ConnectToDevice(IDevice device)
        {
            gattCallback = new GattCallback(this);
            gattCallback.DeviceConnected += NewDeviceConnected;
            gattCallback.DeviceDisconnected += NewDeviceDisconnected;
            Bluetoothgatt1 = ((BluetoothDevice)device.NativeDevice).ConnectGatt(Android.App.Application.Context, false, gattCallback);
        }

        public void DisconnectDevice(IDevice device)
        {
            ((Device)device).Disconnect();
        }

        public void OnLeScan(BluetoothDevice bleDevice, int rssi, byte[] scanRecord)
        {
            Console.WriteLine("Adapter.LeScanCallback: " + bleDevice.Name);
            var device = new Device(bleDevice, null, null, rssi);
            if (DeviceExistsInDiscoveredList(bleDevice) == false)
            {
                discoveredDevices.Add(device);
                DeviceDiscovered(this, new DeviceDiscoveredEventArgs { Device = device });
            }
        }

        public void StartScanningForDevices(Guid serviceUuid)
        {
            StartScanningForDevices();
        }

        public async void StartScanningForDevices()
        {
            Console.WriteLine("Adapter: Starting a scan for devices.");
            discoveredDevices = new List<IDevice>();
            isScanning = true;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                BluetoothLeScanner mLEScanner = blAdapter.BluetoothLeScanner;
                if (NewAdapter == null)
                    NewAdapter = new Adapter2(this);
                mLEScanner.StartScan(NewAdapter);
            }
            else
            {
                //depricated in API level 21
#pragma warning disable CS0618 // Type or member is obsolete
                blAdapter.StartLeScan(this);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            await Task.Delay(10000);
            if (this.isScanning)
            {
                Console.WriteLine("BluetoothLEManager: Scan timeout has elapsed.");
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    BluetoothLeScanner mLEScanner = blAdapter.BluetoothLeScanner;
                    if (NewAdapter == null)
                        NewAdapter = new Adapter2(this);
                    mLEScanner.StopScan(NewAdapter);
                }
                else
                {
                    //depricated in API level 21
#pragma warning disable CS0618 // Type or member is obsolete
                    this.blAdapter.StopLeScan(this);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                this.ScanTimeoutElapsed(this, new EventArgs());
            }
        }

        public void StopScanningForDevices()
        {
            Console.WriteLine("Adapter: Stopping the scan for devices.");
            this.isScanning = false;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                BluetoothLeScanner mLEScanner = blAdapter.BluetoothLeScanner;
                if (NewAdapter == null)
                    NewAdapter = new Adapter2(this);
                mLEScanner.StopScan(NewAdapter);
            }
            else
            {
                //depricated in API level 21
#pragma warning disable CS0618 // Type or member is obsolete
                this.blAdapter.StopLeScan(this);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        public void ConnectToDevicepublic(Device device)
        {
            Bluetoothgatt1 = ((BluetoothDevice)device.NativeDevice).ConnectGatt(Android.App.Application.Context, false, gattCallback);
        }

        protected bool DeviceExistsInDiscoveredList(BluetoothDevice device)
        {
            foreach (var d in discoveredDevices)
            {
                if (device.Address == ((BluetoothDevice)d.NativeDevice).Address)
                    return true;
            }
            return false;
        }

        private void NewDeviceConnected(object sender, BluetoothDeviceConnectionEventArgs e)
        {
            Console.WriteLine("Device Connected! the device Name is :" + e.Device.Name.ToString());
            connectedDevices.Add(e.Device);
            DeviceConnected(this, e);
        }

        private void NewDeviceDisconnected(object sender, BluetoothDeviceConnectionEventArgs e)
        {
            try
            {
                Console.WriteLine("Device Disconnected! the device Name is :" + e.Device.Name.ToString());
                DeviceDisconnected(this, e);
                gattCallback.DeviceConnected -= NewDeviceConnected;
                gattCallback.DeviceDisconnected -= NewDeviceDisconnected;
                gattCallback = null;
            }
            catch
            {
            }
        }

        private class Adapter2 : ScanCallback
        {
            private readonly Adapter Parent;

            public Adapter2(Adapter parent)
            {
                Parent = parent;
            }

            public override void OnBatchScanResults(IList<ScanResult> results)
            {
                base.OnBatchScanResults(results);
            }

            public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
            {
                base.OnScanFailed(errorCode);
            }

            public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
            {
                base.OnScanResult(callbackType, result);
                if (!Parent.DeviceExistsInDiscoveredList(result.Device))
                {
                    Parent.discoveredDevices.Add(new Device(result.Device, null, null, result.Rssi));
                    Parent.DeviceDiscovered(this, new DeviceDiscoveredEventArgs { Device = Parent.discoveredDevices[Parent.discoveredDevices.Count - 1] });
                }
                Console.WriteLine(result.Device.ToString());
            }
        }
    }
}