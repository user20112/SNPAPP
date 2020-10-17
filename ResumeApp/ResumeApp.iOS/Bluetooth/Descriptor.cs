using CoreBluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using System;

namespace NativeFeatures.IOS
{
    public class Descriptor : IDescriptor
    {
        protected string _name = null;
        protected CBDescriptor _nativeDescriptor;

        public Descriptor(CBDescriptor nativeDescriptor)
        {
            this._nativeDescriptor = nativeDescriptor;
        }

        public Guid ID
        {
            get
            {
                return Guid.ParseExact(this._nativeDescriptor.UUID.ToString(), "d");
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