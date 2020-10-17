using ResumeApp.Classes.Bluetooth.LE;
using System;

namespace ResumeApp.Events
{
    public class CharacteristicReadEventArgs : EventArgs
    {
        public CharacteristicReadEventArgs()
        {
        }

        public ICharacteristic Characteristic { get; set; }
    }
}