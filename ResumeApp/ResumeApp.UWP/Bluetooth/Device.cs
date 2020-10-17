using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.UWP.Bluetooth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Bluetooth;

[assembly: Xamarin.Forms.Dependency(typeof(Device))]

namespace ResumeApp.UWP.Bluetooth
{
    public class Device : DeviceBase
    {
        protected IList<IService> _services = new List<IService>();
        protected BluetoothLEDevice nativeDevice;

        public Device(BluetoothLEDevice nativeDevice)
        {
            this.nativeDevice = nativeDevice;
        }

        public override event EventHandler ServicesDiscovered = delegate { };

        public override Guid ID
        {
            get { return ExtractGuid(); }
        }

        public override string Name
        {
            get { return nativeDevice.Name; }
        }

        public override object NativeDevice
        {
            get { return nativeDevice; }
        }

        public override int Rssi
        {
            get { return 0; }
        }

        public override IList<IService> Services
        {
            get { return _services; }
        }

        public override DeviceState State
        {
            get { return this.GetState(); }
        }

        public override void DiscoverServices()
        {
            Windows.Foundation.IAsyncOperation<Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceServicesResult> task = nativeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
            Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceServicesResult result = task.GetResults();
            this._services.Clear();
            if (result.Status == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                foreach (var item in result.Services)
                {
                    Debug.WriteLine("Device.Discovered Service: " + item.DeviceId);
                    this._services.Add(new Service(item));
                }
                ServicesDiscovered?.Invoke(this, new EventArgs());
            }
            else
            {
            }
        }

        public override void Dispose()
        {
            nativeDevice.Dispose();
        }

        protected DeviceState GetState()
        {
            return nativeDevice.ConnectionStatus switch
            {
                BluetoothConnectionStatus.Connected => DeviceState.Connected,
                _ => DeviceState.Disconnected,
            };
        }

        private Guid ExtractGuid()
        {
            return new Guid();
        }
    }
}