using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.UWP.Bluetooth;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

[assembly: Xamarin.Forms.Dependency(typeof(Descriptor))]

namespace ResumeApp.UWP.Bluetooth
{
    public class Descriptor : IDescriptor
    {
        protected string _name = null;
        protected GattDescriptor _nativeDescriptor;

        public Descriptor(GattDescriptor nativeDescriptor)
        {
            this._nativeDescriptor = nativeDescriptor;
        }

        public Guid ID
        {
            get { return _nativeDescriptor.Uuid; }
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
            get { return this._nativeDescriptor as Object; }
        }
    }
}