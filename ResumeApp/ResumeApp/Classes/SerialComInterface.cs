using ResumeApp.Interfaces;
using System;
using System.Text;

namespace ResumeApp.Classes
{
    internal class SerialComInterface : ICommunicationInterface
    {
        public EventHandler<string> OnReceive { get; set; } = delegate { };
        private ISerialCom PlatformSpecificSerial;

        public SerialComInterface(ISerialCom platformSpecificSerial)
        {
            PlatformSpecificSerial = platformSpecificSerial;
            platformSpecificSerial.DataReceived += ReceivedMessage;
            PlatformSpecificSerial.Open();
        }

        private void ReceivedMessage(object sender, EventArgs e)
        {
            OnReceive(this, PlatformSpecificSerial.ReadExisting());
        }

        public void Dispose()
        {
            PlatformSpecificSerial.Close();
        }

        public void WriteByte(byte ToWrite)
        {
            PlatformSpecificSerial.WriteByte(ToWrite);
        }

        public void WriteBytes(byte[] ToWrite)
        {
            PlatformSpecificSerial.Write(Encoding.ASCII.GetString(ToWrite));
        }

        public void WriteString(string ToWrite)
        {
            PlatformSpecificSerial.Write(ToWrite);
        }
    }
}