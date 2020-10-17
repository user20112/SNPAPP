using System;

namespace ResumeApp.Interfaces
{
    internal interface ICommunicationInterface : IDisposable
    {
        void WriteString(string ToWrite);

        void WriteByte(byte ToWrite);

        void WriteBytes(byte[] ToWrite);

        EventHandler<string> OnReceive { get; set; }
    }
}