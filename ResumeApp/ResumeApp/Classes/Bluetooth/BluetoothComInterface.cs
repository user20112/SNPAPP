using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeApp.Classes.Bluetooth
{
    public class BluetoothComInterface : ICommunicationInterface
    {
        private IAdapter Adapter;
        private IDevice Device;
        private const string CHARACTERISTIC_UUID_RX = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";//  the characteristic to write to for mldp
        private const string CHARACTERISTIC_UUID_TX = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";//  the characteristic to write to for mldp
        private const string SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";//  the service to find the characterstic for mldp
        private IService SerialBLEService;
        private ICharacteristic SerialRX;
        private ICharacteristic SerialTX;

        public BluetoothComInterface(IAdapter adapter, IDevice device)
        {
            Adapter = adapter;
            Device = device;
            adapter.DeviceConnected += OnDeviceConnect;
            adapter.DeviceDisconnected += OnDeviceDisconnect;
            Adapter.ConnectToDevice(device);
        }

        private void OnDeviceConnect(object sender, BluetoothDeviceConnectionEventArgs e)
        {
            Device = e.Device;
            Device.ServicesDiscovered += ServicesDiscovered;
            Device.DiscoverServices();
        }

        private void ServicesDiscovered(object sender, EventArgs e)
        {
            Device.ServicesDiscovered -= ServicesDiscovered;
            List<IService> Services = new List<IService>(Device.Services);
            foreach (var service in Services)
            {
                string uuid = service.ID.ToString();
                if (uuid.ToLower().Equals(SERVICE_UUID.ToLower()))
                {
                    SerialBLEService = service;
                    SerialBLEService.CharacteristicsDiscovered += CharacteristicsDiscovered;
                    SerialBLEService.DiscoverCharacteristics();
                    break;
                }
            }
        }

        private void CharacteristicsDiscovered(object sender, EventArgs e)
        {
            SerialBLEService.CharacteristicsDiscovered -= CharacteristicsDiscovered;
            foreach (var characteristic in SerialBLEService.Characteristics)
            {
                if (characteristic.Uuid.ToLower().Equals(CHARACTERISTIC_UUID_RX.ToLower()))
                {
                    SerialRX = characteristic;
                    SerialRX.StartUpdates();
                    SerialRX.ValueUpdated += CharacteristicValueUpdated;
                }
                if (characteristic.Uuid.ToLower().Equals(CHARACTERISTIC_UUID_TX.ToLower()))
                    SerialTX = characteristic;//this is used to write to the device
            }
            bool temp = SerialRX.CanRead;
            bool temp1 = SerialTX.CanRead;
        }

        private string overflow = "";

        private void CharacteristicValueUpdated(object sender, CharacteristicReadEventArgs e)
        {
            overflow += e.Characteristic.StringValue;
            if (overflow.Contains(";"))
            {
                int index = overflow.IndexOf(";") + 1;
                string temp = overflow.Substring(0, index - 1);
                overflow = overflow.Substring(index, overflow.Length - index);
                OnReceive?.Invoke(this, temp);
            }
        }

        private void OnDeviceDisconnect(object sender, BluetoothDeviceConnectionEventArgs e)
        {
            try
            {
                Device.ServicesDiscovered -= ServicesDiscovered;
                SerialBLEService.CharacteristicsDiscovered -= CharacteristicsDiscovered;
            }
            catch
            {
            }
        }

        public EventHandler<string> OnReceive { get; set; } = delegate { };

        public void Dispose()
        {
            OnReceive = delegate { };
            try
            {
                Adapter.Dispose();
                Device.Dispose();
            }
            catch
            {
            }
        }

        public void WriteByte(byte ToWrite)
        {
            SerialTX.Write(new byte[1] { ToWrite });
        }

        public void WriteBytes(byte[] ToWrite)
        {
            SerialTX.Write(ToWrite);
        }

        public void WriteString(string ToWrite)
        {
            SerialTX.Write(ASCIIEncoding.ASCII.GetBytes(ToWrite));
        }
    }
}