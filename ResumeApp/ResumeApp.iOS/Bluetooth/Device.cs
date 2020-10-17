using CoreBluetooth;
using Foundation;
using ResumeApp.Classes.Bluetooth.LE;
using System;
using System.Collections.Generic;

namespace NativeFeatures.IOS
{
    public class Device : DeviceBase
    {
        protected CBPeripheral nativeDevice;
        protected int rssi;
        protected IList<IService> services = new List<IService>();

        public Device(CBPeripheral nativeDevice)
        {
            this.nativeDevice = nativeDevice;
            this.nativeDevice.DiscoveredService += (object sender, NSErrorEventArgs e) =>
            {
                if (this.nativeDevice.Services != null)
                {
                    foreach (CBService s in this.nativeDevice.Services)
                    {
                        Console.WriteLine("Device.Discovered Service: " + s.Description);
                        if (!ServiceExists(s))
                            services.Add(new Service(s, this.nativeDevice));
                    }
                    ServicesDiscovered(this, new EventArgs());
                }
            };
            this.nativeDevice.DiscoveredCharacteristic += NativeDeviceDiscoveredCharacteristic;
        }

        ~Device()
        {
            this.nativeDevice.DiscoveredCharacteristic -= NativeDeviceDiscoveredCharacteristic;
        }

        public override event EventHandler ServicesDiscovered = delegate { };

        public override Guid ID => Guid.ParseExact(nativeDevice.Identifier.AsString(), "d");

        public override string Name => nativeDevice.Name;

        public override object NativeDevice => nativeDevice;

        public override int Rssi => rssi;

        public override IList<IService> Services
        {
            get { return this.services; }
        }

        public override DeviceState State => GetState();

        public void Disconnect()
        {
            Adapter.Current.DisconnectDevice(this);
            this.nativeDevice.Dispose();
        }

        public override void DiscoverServices()
        {
            this.nativeDevice.DiscoverServices();
        }

        public override void Dispose()
        {
        }

        protected DeviceState GetState()
        {
            return this.nativeDevice.State switch
            {
                CBPeripheralState.Connected => DeviceState.Connected,
                CBPeripheralState.Connecting => DeviceState.Connecting,
                CBPeripheralState.Disconnected => DeviceState.Disconnected,
                _ => DeviceState.Disconnected,
            };
        }

        protected bool ServiceExists(CBService service)
        {
            foreach (var s in this.services)
            {
                if (s.ID == Service.ServiceUuidToGuid(service.UUID))
                    return true;
            }
            return false;
        }

        private void NativeDeviceDiscoveredCharacteristic(object sender, CBServiceEventArgs e)
        {
            Console.WriteLine("Device.Discovered Characteristics.");
            //loop through each service, and update the characteristics
            foreach (CBService srv in ((CBPeripheral)sender).Services)
            {
                // if the service has characteristics yet
                if (srv.Characteristics != null)
                {
                    // locate the our new service
                    foreach (var item in this.Services)
                    {
                        // if we found the service
                        if (item.ID == Service.ServiceUuidToGuid(srv.UUID))
                        {
                            item.Characteristics.Clear();
                            // add the discovered characteristics to the particular service
                            foreach (var characteristic in srv.Characteristics)
                            {
                                Console.WriteLine("Characteristic: " + characteristic.Description);
                                Characteristic newChar = new Characteristic(characteristic, this.nativeDevice);
                                item.Characteristics.Add(newChar);
                            }
                            // inform the service that the characteristics have been discovered
                            // TODO: really, we should just be using a notifying collection.
                            (item as Service).OnCharacteristicsDiscovered();
                        }
                    }
                }
            }
        }
    }
}