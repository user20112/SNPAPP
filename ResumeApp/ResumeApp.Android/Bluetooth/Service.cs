using Android.Bluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using System;
using System.Collections.Generic;

namespace NativeFeatures.Droid
{
    public class Service : IService
    {
        protected IList<ICharacteristic> _characteristics;
        protected BluetoothGatt _gatt;
        protected GattCallback _gattCallback;
        protected string _name = null;
        protected BluetoothGattService _nativeService;

        public Service(BluetoothGattService nativeService, BluetoothGatt gatt, GattCallback _gattCallback)
        {
            this._nativeService = nativeService;
            this._gatt = gatt;
            this._gattCallback = _gattCallback;
        }

        public event EventHandler CharacteristicsDiscovered = delegate { };

        public IList<ICharacteristic> Characteristics
        {
            get
            {
                if (this._characteristics == null)
                {
                    this._characteristics = new List<ICharacteristic>();
                    foreach (var item in this._nativeService.Characteristics)
                    {
                        this._characteristics.Add(new Characteristic(item, this._gatt, this._gattCallback));
                    }
                }
                return this._characteristics;
            }
        }

        public Guid ID
        {
            get
            {
                return Guid.ParseExact(this._nativeService.Uuid.ToString(), "d");
            }
        }

        public bool IsPrimary
        {
            get
            {
                return (this._nativeService.Type == GattServiceType.Primary ? true : false);
            }
        }

        public string Name
        {
            get
            {
                if (this._name == null)
                    this._name = KnownServices.Lookup(this.ID).Name;
                return this._name;
            }
        }

        public void DiscoverCharacteristics()
        {
            this.CharacteristicsDiscovered(this, new EventArgs());
        }

        public ICharacteristic FindCharacteristic(KnownCharacteristic characteristic)
        {
            foreach (var item in this._nativeService.Characteristics)
            {
                if (string.Equals(item.Uuid.ToString(), characteristic.ID.ToString()))
                {
                    return new Characteristic(item, this._gatt, this._gattCallback);
                }
            }
            return null;
        }
    }
}