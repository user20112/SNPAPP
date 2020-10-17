namespace ResumeApp.Events
{
    public class DeviceDisconnectedEventArgs
    {
        public object Device;
        public string DisconnectReason;

        public DeviceDisconnectedEventArgs(object device, string reason)
        {
            Device = device;
            DisconnectReason = reason;
        }
    }
}