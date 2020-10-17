using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;

//[assembly: Xamarin.Forms.Dependency(typeof(HIDInterface))]

namespace ResumeApp.UWP.InterfaceImplimentations
{
    public class HIDInterface : IHIDDevice
    {
        private HidDevice NativeDevice;
        private DeviceInformation NativeDeviceInfo;
        private HidInputReport ReadReport;
        private bool WaitingOnRead = false;
        public bool Connected { get; set; } = false;
        public string DeviceId { get { if (NativeDevice != null) return NativeDeviceInfo.Id.ToString(); else return ""; } set { } }
        public string DevicePath { get { if (NativeDevice != null) return NativeDeviceInfo.Id.ToString(); else return ""; } set { } }
        public Guid guid { get; set; }
        private ushort vid = 0;
        private ushort pid = 0;
        private ushort usagePage = 0;
        private ushort usageID = 0;

        public void Dispose()
        {
            NativeDevice.Dispose();
        }

        public void Load(string devicePath)
        {
            string selector = HidDevice.GetDeviceSelector(usagePage, usageID, vid, pid);
            Windows.Foundation.IAsyncOperation<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(selector);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
            DeviceInformationCollection devices = task.GetResults();
            foreach (DeviceInformation device in devices)
            {
                if (device.Id == devicePath)
                {
                    FoundDevice(device);
                    return;
                }
            }
        }

        public byte[] Read()
        {
            WaitingOnRead = true;
            NativeDevice.InputReportReceived += ReadingFinished;
            while (WaitingOnRead)
                Thread.Sleep(20);
            DataReader reader = DataReader.FromBuffer(ReadReport.Data);
            byte[] received = new byte[ReadReport.Data.Length];
            reader.ReadBytes(received);
            return received;
        }

        public string Scan(string DevicePID)
        {
            string selector = HidDevice.GetDeviceSelector(usagePage, usageID, vid, pid);
            Windows.Foundation.IAsyncOperation<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(selector);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
            DeviceInformationCollection devices = task.GetResults();
            if (devices.Count > 0)
                return devices[0].Id;
            return "";
        }

        public List<string> Scan()
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, uint cbToWrite)
        {
            if (buffer[0] != 0)
                buffer = AddToBegining(buffer, 0);
            if (buffer.Length < cbToWrite)
            {
                byte[] temp = new byte[cbToWrite];
                for (int x = 0; x < buffer.Length; x++)
                    temp[x] = buffer[x];
                for (int x = buffer.Length; x < cbToWrite; x++)
                    temp[x] = 0;
                buffer = temp;
            }
            HidOutputReport y = NativeDevice.CreateOutputReport();
            y.Data = buffer.AsBuffer();
            _ = NativeDevice.SendOutputReportAsync(y);
        }

        public void WriteMultiple(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private byte[] AddToBegining(byte[] Array, byte ByteToAdd)
        {
            byte[] newArray = new byte[Array.Length + 1];
            Array.CopyTo(newArray, 1);
            newArray[0] = ByteToAdd;
            return newArray;
        }

        private void FoundDevice(DeviceInformation device)
        {
            NativeDeviceInfo = device;
            var task = HidDevice.FromIdAsync(device.Id, FileAccessMode.ReadWrite);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
            NativeDevice = task.GetResults();
        }

        private void ReadingFinished(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            NativeDevice.InputReportReceived -= ReadingFinished;
            ReadReport = args.Report;
            WaitingOnRead = false;
        }
    }
}