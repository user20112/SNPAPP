using System;
using System.Collections.Generic;

namespace ResumeApp.Interfaces
{
    public interface IHIDDevice
    {
        bool Connected { get; }
        string DeviceId { get; set; }
        string DevicePath { get; set; }
        Guid guid { get; set; }

        void Load(string devicePath);

        byte[] Read();

        string Scan(string DevicePID);

        List<string> Scan();

        void Write(byte[] buffer, uint cbToWrite);

        void WriteMultiple(byte[] buffer);
    }
}