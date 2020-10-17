using ResumeApp.Classes.Bluetooth.LE;
using System;

namespace ResumeApp.Events
{
    public class BluetoothDeviceConnectionEventArgs : EventArgs
    {
        public IDevice Device;
        public string ErrorMessage;

        public BluetoothDeviceConnectionEventArgs() : base()
        { }
    }
}