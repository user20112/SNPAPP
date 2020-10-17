using Android.Bluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NativeFeatures.Droid
{
    public class Device : DeviceBase
    {
        protected BluetoothGatt gatt;
        protected GattCallback gattCallback;
        protected BluetoothDevice nativeDevice;
        protected int rssi;
        protected IList<IService> services = new List<IService>();
        private bool Disposed = false;

        public Device(BluetoothDevice nativeDevice, BluetoothGatt gatt, GattCallback gattCallback, int rssi) : base()
        {
            Disposed = false;
            this.nativeDevice = nativeDevice;
            this.gatt = gatt;
            this.gattCallback = gattCallback;
            this.rssi = rssi;
            // when the services are discovered on the gatt callback, cache them here
            if (this.gattCallback != null)
            {
                this.gattCallback.ServicesDiscovered += (s, e) =>
                {
                    var services = this.gatt.Services;
                    this.services = new List<IService>();
                    foreach (var item in services)
                    {
                        this.services.Add(new Service(item, this.gatt, this.gattCallback));
                    }
                    this.ServicesDiscovered(this, e);
                };
            }
        }

        public override event EventHandler ServicesDiscovered = delegate { };

        public override Guid ID
        {
            get
            {
                Byte[] deviceGuid = new Byte[16];
                String macWithoutColons = nativeDevice.Address.Replace(":", "");
                Byte[] macBytes = Enumerable.Range(0, macWithoutColons.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => System.Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                    .ToArray();
                macBytes.CopyTo(deviceGuid, 10);
                return new Guid(deviceGuid);
            }
        }

        public override string Name { get { return nativeDevice.Name; } }

        public override object NativeDevice { get { return nativeDevice; } }

        public override int Rssi { get { return rssi; } }

        public override IList<IService> Services
        {
            get { return services; }
        }

        public override DeviceState State { get { return GetState(); } }

        public void Disconnect()
        {
            Disposed = true;
            gatt?.Disconnect();
        }

        public override void DiscoverServices()
        {
            if (!Disposed)
                gatt.DiscoverServices();
        }

        public override void Dispose()
        {
            nativeDevice.Dispose();
        }

        protected DeviceState GetState()
        {
            return this.nativeDevice.BondState switch
            {
                Bond.Bonded => DeviceState.Connected,
                Bond.Bonding => DeviceState.Connecting,
                _ => DeviceState.Disconnected,
            };
            ;
        }
    }
}