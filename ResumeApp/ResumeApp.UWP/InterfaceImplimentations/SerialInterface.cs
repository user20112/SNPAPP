using ResumeApp.Interfaces;
using System;
using System.Collections.Generic;

namespace ResumeApp.UWP.InterfaceImplimentations
{
    internal class SerialInterface : ISerialCom
    {
        public EventHandler<EventArgs> DataReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOpen => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Load(string id)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public string ReadExisting()
        {
            throw new NotImplementedException();
        }

        public List<string> Scan()
        {
            throw new NotImplementedException();
        }

        public void SetBaud(int rate)
        {
            throw new NotImplementedException();
        }

        public void SetData(int bits)
        {
            throw new NotImplementedException();
        }

        public void SetParity(string type)
        {
            throw new NotImplementedException();
        }

        public void SetStop(double bits)
        {
            throw new NotImplementedException();
        }

        public void Write(string ToWrite)
        {
            throw new NotImplementedException();
        }

        public void WriteByte(byte ToWrite)
        {
            throw new NotImplementedException();
        }
    }
}