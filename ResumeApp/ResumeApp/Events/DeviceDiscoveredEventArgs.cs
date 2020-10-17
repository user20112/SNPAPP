using ResumeApp.Classes.Bluetooth.LE;
using System;

namespace ResumeApp.Events
{
    public class DeviceDiscoveredEventArgs : EventArgs
    {
        public IDevice Device;

        public DeviceDiscoveredEventArgs() : base()
        { }
    }
}