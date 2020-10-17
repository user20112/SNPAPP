using ResumeApp.Interfaces;
using ResumeApp.WPF.Classes;
using ResumeApp.WPF.InterfaceImplimentations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(HIDDeviceInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class HIDDeviceInterface : IHIDDevice
    {
        private Guid _guid = new Guid("4d1e55b2-f16f-11cf-88cb-001111000030");

        public HIDDeviceInterface()
        {
            DeviceId = "";
        }

        public bool Connected
        {
            get
            {
                return DevicePath != null;
            }
        }
        public string DeviceId { get; set; }
        public string DevicePath { get; set; }
        public Guid guid { get { return _guid; } set { _guid = value; } }

        public void Dispose()
        {
        }

        public void Load(string devicePath)
        {
            DevicePath = devicePath;
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[65];
            using (HIDFile ReadFile = new HIDFile(
                this.DevicePath,
                HIDFile.DesiredAccess.GENERIC_READ,
                HIDFile.ShareMode.FILE_SHARE_READ | HIDFile.ShareMode.FILE_SHARE_WRITE,
                HIDFile.CreationDisposition.OPEN_EXISTING))
                ReadFile.Read(buffer, (uint)buffer.Length);
            return buffer;
        }

        public string Scan(string DevicePID)
        {
            string deviceId = DeviceId + "&" + DevicePID;
            WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA();
            WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA();
            WindowsSetupAPI.SP_DEVINFO_DATA deviceInfoData = new WindowsSetupAPI.SP_DEVINFO_DATA();
            uint interfaceIndex = 0;
            bool matchFound;
            //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID.
            IntPtr pDeviceInfoTable = WindowsSetupAPI.SetupDiGetClassDevs(
                ref _guid,
                null,
                IntPtr.Zero,
                WindowsSetupAPI.DIGCF_PRESENT | WindowsSetupAPI.DIGCF_DEVICEINTERFACE
            );
            try
            {
                bool looping = true;
                while (looping)
                {
                    // loop through checking for the device
                    deviceInterfaceData.cbSize = (UInt32)Marshal.SizeOf(typeof(WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA));
                    if (!WindowsSetupAPI.SetupDiEnumDeviceInterfaces(pDeviceInfoTable, IntPtr.Zero, ref _guid, interfaceIndex, ref deviceInterfaceData))
                    {
                        int error = Marshal.GetLastWin32Error();
                        return "";
                    }
                    //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then
                    //check to see if it is the correct device or not.
                    //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                    deviceInfoData.cbSize = (UInt32)Marshal.SizeOf(typeof(WindowsSetupAPI.SP_DEVINFO_DATA));
                    if (!WindowsSetupAPI.SetupDiEnumDeviceInfo(pDeviceInfoTable, interfaceIndex, ref deviceInfoData))
                    {
                        return "";
                    }
                    //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                    //SetupDiGetDeviceRegistryPropertyUM(DeviceInfoTable, &DevInfoData, SPDRP_HARDWAREID, &dwRegType, NULL, 0, &dwRegSize);
                    WindowsSetupAPI.SetupDiGetDeviceRegistryProperty(
                        pDeviceInfoTable,
                        ref deviceInfoData,
                        WindowsSetupAPI.SPDRP_HARDWAREID,
                        out uint dwRegType, null, 0, out uint dwRegSize);
                    //Allocate a buffer for the hardware ID.
                    byte[] propertyValueBuffer = new byte[dwRegSize];
                    //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a
                    //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                    //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example
                    //format "Vid_04d8&Pid_003f".
                    if (!WindowsSetupAPI.SetupDiGetDeviceRegistryProperty(pDeviceInfoTable, ref deviceInfoData, WindowsSetupAPI.SPDRP_HARDWAREID, out dwRegType, propertyValueBuffer, dwRegSize, out dwRegSize))
                    {
                        return "";
                    }
                    //Now check if the first string in the hardware ID matches the device ID of my USB device.
                    String deviceIdFromRegistry = Encoding.Unicode.GetString(propertyValueBuffer);
                    //Convert both strings to lower case.  This makes the code more robust/portable across OS Versions
                    deviceIdFromRegistry = deviceIdFromRegistry.ToLowerInvariant();
                    string deviceIdToFind = deviceId.ToLowerInvariant();
                    //Now check if the hardware ID we are looking at contains the correct VID/PID
                    matchFound = deviceIdFromRegistry.Contains(deviceIdToFind);
                    if (matchFound)
                    {
                        //Device has been found. now we need to get the device path. once we have that we can return that and everything will go smoothly.
                        deviceInterfaceDetailData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        if (IntPtr.Size == 8) // for 64 bit operating systems
                            deviceInterfaceDetailData.cbSize = 8;
                        else
                            deviceInterfaceDetailData.cbSize = (UInt32)(4 + Marshal.SystemDefaultCharSize); // for 32 bit systems
                        UInt32 bufferSize = 1000;
                        if (!WindowsSetupAPI.SetupDiGetDeviceInterfaceDetail(pDeviceInfoTable, ref deviceInterfaceData, ref deviceInterfaceDetailData, bufferSize, out uint structureSize, ref deviceInfoData))
                        {
                            return "";
                        }
                        if (deviceInterfaceDetailData.DevicePath != null)
                        {
                            return deviceInterfaceDetailData.DevicePath;//we sucessfully found the device!
                        }
                    }
                    interfaceIndex++;
                    if (interfaceIndex == 100) //Im limiting this to 100 it was 10 million before im not sure if this is to little but i doubt it will be.
                    {
                        looping = false;
                    }
                    //Keep looping until we either find a device with matching VID and PID, or until we run out of items, or some error is encountered.
                }
                return "";
            }
            finally
            {
                //Clean up the old structure we no longer need.
                WindowsSetupAPI.SetupDiDestroyDeviceInfoList(pDeviceInfoTable);
            }
        }

        public List<string> Scan()
        {
            List<string> ReturnValue = new List<string>();
            WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA();
            WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA();
            WindowsSetupAPI.SP_DEVINFO_DATA deviceInfoData = new WindowsSetupAPI.SP_DEVINFO_DATA();
            uint interfaceIndex = 0;
            bool matchFound;
            //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID.
            IntPtr pDeviceInfoTable = WindowsSetupAPI.SetupDiGetClassDevs(
                ref _guid,
                null,
                IntPtr.Zero,
                WindowsSetupAPI.DIGCF_PRESENT | WindowsSetupAPI.DIGCF_DEVICEINTERFACE
            );
            try
            {
                bool looping = true;
                while (looping)
                {
                    // loop through checking for the device
                    deviceInterfaceData.cbSize = (UInt32)Marshal.SizeOf(typeof(WindowsSetupAPI.SP_DEVICE_INTERFACE_DATA));
                    if (!WindowsSetupAPI.SetupDiEnumDeviceInterfaces(pDeviceInfoTable, IntPtr.Zero, ref _guid, interfaceIndex, ref deviceInterfaceData))
                    {
                        int error = Marshal.GetLastWin32Error();
                        return ReturnValue;
                    }
                    //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then
                    //check to see if it is the correct device or not.
                    //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                    deviceInfoData.cbSize = (UInt32)Marshal.SizeOf(typeof(WindowsSetupAPI.SP_DEVINFO_DATA));
                    if (!WindowsSetupAPI.SetupDiEnumDeviceInfo(pDeviceInfoTable, interfaceIndex, ref deviceInfoData))
                    {
                        return ReturnValue;
                    }
                    //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                    //SetupDiGetDeviceRegistryPropertyUM(DeviceInfoTable, &DevInfoData, SPDRP_HARDWAREID, &dwRegType, NULL, 0, &dwRegSize);
                    WindowsSetupAPI.SetupDiGetDeviceRegistryProperty(
                        pDeviceInfoTable,
                        ref deviceInfoData,
                        WindowsSetupAPI.SPDRP_HARDWAREID,
                        out uint dwRegType, null, 0, out uint dwRegSize);
                    //Allocate a buffer for the hardware ID.
                    byte[] propertyValueBuffer = new byte[dwRegSize];
                    //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a
                    //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                    //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example
                    //format "Vid_04d8&Pid_003f".
                    if (!WindowsSetupAPI.SetupDiGetDeviceRegistryProperty(pDeviceInfoTable, ref deviceInfoData, WindowsSetupAPI.SPDRP_HARDWAREID, out dwRegType, propertyValueBuffer, dwRegSize, out dwRegSize))
                    {
                        return ReturnValue;
                    }
                    //Now check if the first string in the hardware ID matches the device ID of my USB device.
                    String deviceIdFromRegistry = Encoding.Unicode.GetString(propertyValueBuffer);
                    //Convert both strings to lower case.  This makes the code more robust/portable across OS Versions
                    deviceIdFromRegistry = deviceIdFromRegistry.ToLowerInvariant();
                    deviceInterfaceDetailData = new WindowsSetupAPI.SP_DEVICE_INTERFACE_DETAIL_DATA();
                    if (IntPtr.Size == 8) // for 64 bit operating systems
                        deviceInterfaceDetailData.cbSize = 8;
                    else
                        deviceInterfaceDetailData.cbSize = (UInt32)(4 + Marshal.SystemDefaultCharSize); // for 32 bit systems
                    UInt32 bufferSize = 1000;
                    if (WindowsSetupAPI.SetupDiGetDeviceInterfaceDetail(pDeviceInfoTable, ref deviceInterfaceData, ref deviceInterfaceDetailData, bufferSize, out uint structureSize, ref deviceInfoData))
                    {
                        if (deviceInterfaceDetailData.DevicePath != null)
                        {
                            ReturnValue.Add(deviceInterfaceDetailData.DevicePath);
                        }
                    }
                    interfaceIndex++;
                    if (interfaceIndex == 100) //Im limiting this to 100 it was 10 million before im not sure if this is to little but i doubt it will be.
                    {
                        looping = false;
                    }
                }
                return ReturnValue;
            }
            finally
            {
                //Clean up the old structure we no longer need.
                WindowsSetupAPI.SetupDiDestroyDeviceInfoList(pDeviceInfoTable);
            }
        }

        public void Write(byte[] buffer, uint cbToWrite)
        {
            if (buffer[0] != 0)
                buffer = AddToBegining(buffer, 0);
            using (HIDFile WriteFile = new HIDFile(
                this.DevicePath,
                HIDFile.DesiredAccess.GENERIC_WRITE,
                HIDFile.ShareMode.FILE_SHARE_READ | HIDFile.ShareMode.FILE_SHARE_WRITE,
                HIDFile.CreationDisposition.OPEN_EXISTING))
                WriteFile.Write(buffer, 65);
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
    }
}