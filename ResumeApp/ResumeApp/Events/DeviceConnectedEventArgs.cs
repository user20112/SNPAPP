namespace ResumeApp.Events
{
    public class DeviceConnectedEventArgs
    {
        public string ConnectString;
        public object Sender;

        public DeviceConnectedEventArgs(object sender, string reConnectString)
        {
            Sender = sender;
            ConnectString = reConnectString;
        }
    }
}