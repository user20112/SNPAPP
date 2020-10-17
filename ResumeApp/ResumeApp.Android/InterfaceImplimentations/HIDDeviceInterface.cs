using Android.Content;
using Android.Hardware.Usb;
using Java.Nio;
using ResumeApp.Droid.InterfaceImplimentations;
using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(HIDDeviceInterface))]

namespace ResumeApp.Droid.InterfaceImplimentations
{
    public class HIDDeviceInterface : IHIDDevice
    {
        public SynchronizationContext Main;
        private UsbDeviceConnection Connection;
        private Context context;
        private ICollection<UsbDevice> DeviceList;
        private UsbEndpoint Endpoint;
        private UsbEndpoint EndpointWrite;
        private bool LoadReattempt = true;
        private UsbManager Manager;
        private UsbDevice NativeDevice;

        public HIDDeviceInterface()
        {
            Main = SynchronizationContext.Current;
            context = MainActivity.Instance;
            Manager = (UsbManager)context.GetSystemService(Context.UsbService);
        }

        public bool Connected { get; set; } = false;
        public string DeviceId { get { if (NativeDevice != null) return NativeDevice.DeviceId.ToString(); else return ""; } set { } }
        public string DevicePath { get { if (NativeDevice != null) return NativeDevice.DeviceName.ToString(); else return ""; } set { } }
        public Guid guid { get; set; }

        public void Dispose()
        {
            if (NativeDevice != null)
            {
                NativeDevice.Dispose();
                NativeDevice = null;
            }
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
            if (Endpoint != null)
            {
                Endpoint.Dispose();
                Endpoint = null;
            }
            if (EndpointWrite != null)
            {
                EndpointWrite.Dispose();
                EndpointWrite = null;
            }
            if (Manager != null)
            {
                Manager.Dispose();
                Manager = null;
            }
        }

        public void Load(string devicePath)
        {
            Manager = (UsbManager)context.GetSystemService(Context.UsbService);
            IDictionary<String, UsbDevice> deviceList = Manager.DeviceList;
            DeviceList = deviceList.Values;
            try
            {
                foreach (UsbDevice device in DeviceList)
                {
                    if (device.DeviceName == devicePath)
                    {
                        if (Manager.HasPermission(device))
                        {
                            Connected = true;
                            NativeDevice = device;
                            Connection = Manager.OpenDevice(NativeDevice);
                            Endpoint = NativeDevice.GetInterface(0).GetEndpoint(0);
                            EndpointWrite = NativeDevice.GetInterface(0).GetEndpoint(1);
                            Connection.ClaimInterface(NativeDevice.GetInterface(0), true);
                        }
                        else
                        {
                            if (LoadReattempt)
                            {
                                LoadReattempt = false;
                                bool Granted = MainActivity.Instance.RequestUSBPermissions(Manager, device, Main);
                                Load(devicePath);
                            }
                            LoadReattempt = true;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public byte[] Read()
        {
            byte[] ReceivedBuffer = new byte[65];
            if (Connection != null)
            {
                if (Endpoint != null)
                {
                    Connection.BulkTransfer(Endpoint, ReceivedBuffer, 1, 64, 300);
                }
            }
            return ReceivedBuffer;
        }

        public string Scan(string DevicePID)
        {
            Manager = (UsbManager)context.GetSystemService(Context.UsbService);
            IDictionary<String, UsbDevice> deviceList = Manager.DeviceList;
            DeviceList = deviceList.Values;
            foreach (UsbDevice device in DeviceList)
            {
                string DeviceID = ("vid_" + device.VendorId.ToString("X4") + "&pid_" + device.ProductId.ToString("X4")).ToLower();
                if (DeviceID.Contains(DevicePID))
                {
                    return device.DeviceName;
                }
            }
            return "";
        }

        public List<string> Scan()
        {
            Manager = (UsbManager)context.GetSystemService(Context.UsbService);
            IDictionary<String, UsbDevice> deviceList = Manager.DeviceList;
            DeviceList = deviceList.Values;
            List<string> ReturnValue = new List<string>();
            foreach (UsbDevice device in DeviceList)
            {
                string DeviceID = ("vid_" + device.VendorId.ToString("X4") + "&pid_" + device.ProductId.ToString("X4")).ToLower();
                ReturnValue.Add(DeviceID);
            }
            return ReturnValue;
        }

        public void Write(byte[] buffer, uint cbToWrite)
        {
            if (Connection != null)
                if (EndpointWrite != null)
                {
                    if (buffer.Length < cbToWrite)
                    {
                        byte[] temp = new byte[cbToWrite];
                        buffer.CopyTo(temp, 0);
                        buffer = temp;
                    }
                    if (buffer[0] == 0)
                    {
                        cbToWrite--;
                        byte[] temp = new byte[cbToWrite];
                        for (int x = 0; x < cbToWrite; x++)
                        {
                            temp[x] = buffer[x + 1];
                        }
                        buffer = temp;
                    }
                    UsbRequest request = new UsbRequest();
                    ByteBuffer bytes644 = ByteBuffer.Wrap(buffer);
                    request.Initialize(Connection, EndpointWrite);
                    while (!request.Queue(bytes644, buffer.Length))
                        Thread.Sleep(40);
                    if (Connection.RequestWait() == request)
                    {
                        Console.WriteLine("Changed Wavelength");
                    }
                }
        }

        public void WriteMultiple(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}