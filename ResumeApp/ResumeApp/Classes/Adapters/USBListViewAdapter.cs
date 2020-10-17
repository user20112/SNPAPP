using ResumeApp.Interfaces;

namespace ResumeApp.Classes.Adapters
{
    public class USBListViewAdapter
    {
        public IHIDDevice BaseDevice;
        public string DeviceID;
        public string CurrentValue;

        public USBListViewAdapter(IHIDDevice baseDevice)
        {
            BaseDevice = baseDevice;
        }
    }
}