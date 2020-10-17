using CoreBluetooth;
using CoreFoundation;
using NativeFeatures.IOS;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Adapter))]

namespace NativeFeatures.IOS
{
    public class Adapter : IAdapter
    {
        protected CBCentralManager _central;
        protected IList<IDevice> _connectedDevices = new List<IDevice>();
        protected IList<IDevice> _discoveredDevices = new List<IDevice>();
        protected bool _isConnecting;
        protected bool _isScanning;
        private static readonly Adapter _current;
        private readonly AutoResetEvent stateChanged = new AutoResetEvent(false);

        static Adapter()
        {
            _current = new Adapter();
        }

        public Adapter()
        {
            this._central = new CBCentralManager(DispatchQueue.CurrentQueue);
            _central.DiscoveredPeripheral += (object sender, CBDiscoveredPeripheralEventArgs e) =>
            {
                Console.WriteLine("DiscoveredPeripheral: " + e.Peripheral.Name);
                Device d = new Device(e.Peripheral);
                if (!ContainsDevice(this._discoveredDevices, e.Peripheral))
                {
                    this._discoveredDevices.Add(d);
                    this.DeviceDiscovered(this, new DeviceDiscoveredEventArgs() { Device = d });
                }
            };
            _central.UpdatedState += (object sender, EventArgs e) =>
            {
                Console.WriteLine("UpdatedState: " + _central.State);
                stateChanged.Set();
            };
            _central.ConnectedPeripheral += (object sender, CBPeripheralEventArgs e) =>
            {
                Console.WriteLine("ConnectedPeripheral: " + e.Peripheral.Name);
                // when a peripheral gets connected, add that peripheral to our running list of connected peripherals
                if (!ContainsDevice(this._connectedDevices, e.Peripheral))
                {
                    Device d = new Device(e.Peripheral);
                    this._connectedDevices.Add(new Device(e.Peripheral));
                    // raise our connected event
                    this.DeviceConnected(sender, new BluetoothDeviceConnectionEventArgs() { Device = d });
                }
            };
            _central.DisconnectedPeripheral += (object sender, CBPeripheralErrorEventArgs e) =>
            {
                Console.WriteLine("DisconnectedPeripheral: " + e.Peripheral.Name);
                // when a peripheral disconnects, remove it from our running list.
                IDevice foundDevice = null;
                foreach (var d in this._connectedDevices)
                {
                    if (d.ID == Guid.ParseExact(e.Peripheral.Identifier.AsString(), "d"))
                        foundDevice = d;
                }
                if (foundDevice != null)
                    this._connectedDevices.Remove(foundDevice);
                //// raise our disconnected event
                this.DeviceDisconnected(sender, new BluetoothDeviceConnectionEventArgs());
                // raise our disconnected event
                //this.DeviceDisconnected (sender, null);
            };
            _central.FailedToConnectPeripheral += (object sender, CBPeripheralErrorEventArgs e) =>
            {
                // raise the Failed to connect event
                this.DeviceFailedToConnect(this, new BluetoothDeviceConnectionEventArgs()
                {
                    Device = new Device(e.Peripheral),
                    ErrorMessage = e.Error.Description
                });
            };
        }

        public event EventHandler ConnectTimeoutElapsed = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceConnected = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceDisconnected = delegate { };

        // events
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceFailedToConnect = delegate { };

        public event EventHandler ScanTimeoutElapsed = delegate { };

        public static Adapter Current
        { get { return _current; } }

        public CBCentralManager Central
        { get { return this._central; } }

        public IList<IDevice> ConnectedDevices
        {
            get
            {
                return this._connectedDevices;
            }
        }

        public IList<IDevice> DiscoveredDevices
        {
            get
            {
                return this._discoveredDevices;
            }
        }

        public bool IsConnecting
        {
            get { return this._isConnecting; }
        }

        public bool IsScanning
        {
            get { return this._isScanning; }
        }

        public void ConnectToDevice(IDevice device)
        {
            this._central.ConnectPeripheral(device.NativeDevice as CBPeripheral, new PeripheralConnectionOptions());
        }

        public void DisconnectDevice(IDevice device)
        {
            Console.WriteLine("Trying to disconnect " + device.Name);
            Console.WriteLine("Current State: " + device.State);
            this._central.CancelPeripheralConnection(device.NativeDevice as CBPeripheral);
            Console.WriteLine("After attempting to disconnect " + device.Name);
            Console.WriteLine("New State: " + device.State);
        }

        public void Dispose()
        {
        }

        public void StartScanningForDevices()
        {
            StartScanningForDevices(serviceUuid: Guid.Empty);
        }

        public async void StartScanningForDevices(Guid serviceUuid)
        {
            await WaitForState(CBCentralManagerState.PoweredOn);
            Debug.WriteLine("Adapter: Starting a scan for devices.");
            CBUUID[] serviceUuids = null; // TODO: convert to list so multiple Uuids can be detected
            if (serviceUuid != Guid.Empty)
            {
                var suuid = CBUUID.FromString(serviceUuid.ToString());
                serviceUuids = new CBUUID[] { suuid };
                Debug.WriteLine("Adapter: Scanning for " + suuid);
            }
            this._discoveredDevices = new List<IDevice>();
            this._isScanning = true;
            this._central.ScanForPeripherals(serviceUuids);
            await Task.Delay(10000);
            if (this._isScanning)
            {
                Console.WriteLine("BluetoothLEManager: Scan timeout has elapsed.");
                this._isScanning = false;
                this._central.StopScan();
                this.ScanTimeoutElapsed(this, new EventArgs());
            }
        }

        public void StopScanningForDevices()
        {
            Console.WriteLine("Adapter: Stopping the scan for devices.");
            this._isScanning = false;
            this._central.StopScan();
        }

        // util
        protected bool ContainsDevice(IEnumerable<IDevice> list, CBPeripheral device)
        {
            foreach (var d in list)
            {
                if (Guid.ParseExact(device.Identifier.AsString(), "d") == d.ID)
                    return true;
            }
            return false;
        }

        private async Task WaitForState(CBCentralManagerState state)
        {
            Debug.WriteLine("Adapter: Waiting for state: " + state);
            while (_central.State != state)
            {
                await Task.Run(() => stateChanged.WaitOne());
            }
        }
    }
}