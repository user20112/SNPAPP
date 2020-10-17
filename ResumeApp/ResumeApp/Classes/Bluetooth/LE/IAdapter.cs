using ResumeApp.Events;
using System;
using System.Collections.Generic;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public interface IAdapter : IDisposable
    {
        event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceConnected;

        event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceDisconnected;

        // events
        event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;

        //TODO: add this
        //event EventHandler<DeviceConnectionEventArgs> DeviceFailedToConnect;
        event EventHandler ScanTimeoutElapsed;

        //TODO: add this
        //event EventHandler ConnectTimeoutElapsed;
        IList<IDevice> ConnectedDevices { get; }

        IList<IDevice> DiscoveredDevices { get; }

        // properties
        bool IsScanning { get; }

        void ConnectToDevice(IDevice device);

        void DisconnectDevice(IDevice device);

        // methods
        void StartScanningForDevices();

        void StartScanningForDevices(Guid serviceUuid);

        void StopScanningForDevices();
    }
}