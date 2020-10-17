using Android.Bluetooth;
using Android.Runtime;
using ResumeApp.Events;
using System;

namespace NativeFeatures.Droid
{
    public class GattCallback : BluetoothGattCallback
    {
        protected Adapter _adapter;

        public GattCallback(Adapter adapter)
        {
            this._adapter = adapter;
        }

        public event EventHandler<CharacteristicReadEventArgs> CharacteristicValueUpdated = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceConnected = delegate { };

        public event EventHandler<BluetoothDeviceConnectionEventArgs> DeviceDisconnected = delegate { };

        public event EventHandler<ServicesDiscoveredEventArgs> ServicesDiscovered = delegate { };

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            this.CharacteristicValueUpdated(this, new CharacteristicReadEventArgs()
            {
                Characteristic = new Characteristic(characteristic, gatt, this)
            }
            );
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            Device device = new Device(gatt.Device, gatt, this, 0);
            switch (newState)
            {
                case ProfileState.Disconnected:
                    _adapter.disconnectCount = 0;
                    this.DeviceDisconnected(this, new BluetoothDeviceConnectionEventArgs() { Device = device });
                    CharacteristicValueUpdated = delegate { };
                    break;

                case ProfileState.Connecting:
                    break;

                case ProfileState.Connected:
                    this.DeviceConnected(this, new BluetoothDeviceConnectionEventArgs() { Device = device });
                    break;

                case ProfileState.Disconnecting:
                    break;
            }
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorRead(gatt, descriptor, status);
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            ServicesDiscovered(this, new ServicesDiscoveredEventArgs());
        }
    }
}