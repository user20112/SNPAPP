using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using ResumeApp.UWP.Bluetooth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(Adapter))]

namespace ResumeApp.UWP.Bluetooth
{
    public class Adapter : IAdapter
    {
        protected IList<IDevice> connectedDevices = new List<IDevice>();
        protected IList<IDevice> discoveredDevices = new List<IDevice>();
        protected bool isScanning;
        private DeviceWatcher deviceWatcher;

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceConnected;

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceDisconnected;

        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;

        public event EventHandler ScanTimeoutElapsed;

        public IList<IDevice> ConnectedDevices
        {
            get { return connectedDevices; }
        }

        public IList<IDevice> DiscoveredDevices
        {
            get { return discoveredDevices; }
        }

        public bool IsScanning
        {
            get { return isScanning; }
        }

        public void ConnectToDevice(IDevice device)
        {
            this.connectedDevices.Add(device);
            DeviceConnected(this, new BluetoothDeviceConnectionEventArgs() { Device = device, ErrorMessage = "error" });
        }

        public void DisconnectDevice(IDevice device)
        {
            this.connectedDevices.Remove(device);
        }

        public void Dispose()
        {
            foreach (IDevice device in ConnectedDevices)
            {
                DisconnectDevice(device);
                device.Dispose();
            }
        }

        public void StartScanningForDevices()
        {
            StartScanningForDevices(serviceUuid: Guid.Empty);
        }

        public void StartScanningForDevices(Guid serviceUuid)
        {
            if (isScanning == true)
                return;
            isScanning = true;
            discoveredDevices = new List<IDevice>();
            Debug.WriteLine("Adapter: Starting a scan for devices.");
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
            deviceWatcher = DeviceInformation.CreateWatcher(
                        aqsAllBluetoothLEDevices,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;
            deviceWatcher.Start();
            isScanning = false;
        }

        public void StopScanningForDevices()
        {
            this.isScanning = false;
            if (deviceWatcher != null)
                deviceWatcher.Stop();
        }

        protected bool DeviceExistsInDiscoveredList(BluetoothLEDevice device)
        {
            foreach (var d in discoveredDevices)
            {
                if (device.BluetoothAddress == ((BluetoothLEDevice)d.NativeDevice).BluetoothAddress)
                    return true;
            }
            return false;
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Windows.Foundation.IAsyncOperation<BluetoothLEDevice> task = BluetoothLEDevice.FromIdAsync(args.Id);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
            Device d = new Device(task.GetResults());
            discoveredDevices.Add(d);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DeviceDiscovered(this, new DeviceDiscoveredEventArgs() { Device = d });
            });
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            StopScanningForDevices();
            ScanTimeoutElapsed?.Invoke(this, new EventArgs());
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }
    }
}