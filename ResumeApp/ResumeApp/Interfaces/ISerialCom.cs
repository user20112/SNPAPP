using System;
using System.Collections.Generic;

namespace ResumeApp.Interfaces
{
    public interface ISerialCom
    {
        EventHandler<EventArgs> DataReceived { get; set; }
        bool IsOpen { get; }

        public void SetBaud(int rate);

        public void SetData(int bits);

        public void SetParity(string type);

        public void SetStop(double bits);

        void Close();

        void Open();

        byte ReadByte();

        string ReadExisting();

        void Write(string ToWrite);

        void WriteByte(byte ToWrite);

        List<string> Scan();

        void Load(string id);
    }
}