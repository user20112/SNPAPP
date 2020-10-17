using ResumeApp.Interfaces;
using ResumeApp.WPF.InterfaceImplimentations;
using System;
using System.Collections.Generic;
using System.IO.Ports;

[assembly: Xamarin.Forms.Dependency(typeof(SerialInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    internal class SerialInterface : ISerialCom
    {
        private SerialPort UnderlyingPort;

        public SerialInterface()
        {
        }

        public EventHandler<EventArgs> DataReceived { get; set; }
        public bool IsOpen { get { if (UnderlyingPort != null) return UnderlyingPort.IsOpen; else return false; } }

        public void Close()
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    UnderlyingPort.Close();
        }

        public void Load(string id)
        {
            UnderlyingPort = new SerialPort(id);
            UnderlyingPort.DataReceived += PortDataReceived;
        }

        public void Open()
        {
            if (UnderlyingPort != null)
                if (!UnderlyingPort.IsOpen)
                    UnderlyingPort.Open();
        }

        public byte ReadByte()
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    return (byte)UnderlyingPort.ReadByte();
            return 0;
        }

        public string ReadExisting()
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    return UnderlyingPort.ReadExisting();
            return "";
        }

        public List<string> Scan()
        {
            return new List<string>(SerialPort.GetPortNames());
        }

        public void SetBaud(int rate)
        {
            UnderlyingPort.BaudRate = rate;
        }

        public void SetData(int bits)
        {
            UnderlyingPort.DataBits = bits;
        }

        public void SetParity(string type)
        {
            switch (type)
            {
                case "Even": UnderlyingPort.Parity = Parity.Even; break;
                case "Mark": UnderlyingPort.Parity = Parity.Mark; break;
                case "None": UnderlyingPort.Parity = Parity.None; break;
                case "Odd": UnderlyingPort.Parity = Parity.Odd; break;
                case "Space": UnderlyingPort.Parity = Parity.Space; break;
            }
        }

        public void SetStop(double bits)
        {
            switch (bits)
            {
                case 0:
                    UnderlyingPort.StopBits = StopBits.None; break;
                case 1:
                    UnderlyingPort.StopBits = StopBits.One; break;
                case 1.5:
                    UnderlyingPort.StopBits = StopBits.OnePointFive; break;
                case 2:
                    UnderlyingPort.StopBits = StopBits.Two; break;
            }
        }

        public void Write(string ToWrite)
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    UnderlyingPort.Write(ToWrite);
        }

        public void WriteByte(byte ToWrite)
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    UnderlyingPort.Write(new byte[1] { ToWrite }, 0, 1);
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (UnderlyingPort != null)
                if (UnderlyingPort.IsOpen)
                    DataReceived?.Invoke(this, new EventArgs());
        }
    }
}