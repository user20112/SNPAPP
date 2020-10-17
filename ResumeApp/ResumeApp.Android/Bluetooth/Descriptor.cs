using Android.Bluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using System;

namespace ResumeApp.Bluetooth.LE
{
    public class Descriptor : IDescriptor
    {
        protected string _name = null;
        protected BluetoothGattDescriptor _nativeDescriptor;

        public Descriptor(BluetoothGattDescriptor nativeDescriptor)
        {
            this._nativeDescriptor = nativeDescriptor;
        }

        public Guid ID
        {
            get
            {
                return Guid.ParseExact(this._nativeDescriptor.Uuid.ToString(), "d");
            }
        }

        public string Name
        {
            get
            {
                if (this._name == null)
                    this._name = KnownDescriptors.Lookup(this.ID).Name;
                return this._name;
            }
        }

        public object NativeDescriptor
        {
            get
            {
                return this._nativeDescriptor as object;
            }
        }
    }
}