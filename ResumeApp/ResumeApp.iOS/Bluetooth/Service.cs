using CoreBluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using System;
using System.Collections.Generic;

namespace NativeFeatures.IOS
{
    public class Service : IService
    {
        protected IList<ICharacteristic> characteristics;
        protected string name = null;
        protected CBService nativeService;
        protected CBPeripheral parentDevice;

        public Service(CBService nativeService, CBPeripheral parentDevice)
        {
            this.nativeService = nativeService;
            this.parentDevice = parentDevice;
        }

        public event EventHandler CharacteristicsDiscovered = delegate { };

        public IList<ICharacteristic> Characteristics
        {
            get
            {
                // if it hasn't been populated yet, populate it
                if (characteristics == null)
                {
                    characteristics = new List<ICharacteristic>();
                    if (nativeService.Characteristics != null)
                    {
                        foreach (var item in this.nativeService.Characteristics)
                            characteristics.Add(new Characteristic(item, parentDevice));
                    }
                }
                return this.characteristics;
            }
        }

        public Guid ID { get { return ServiceUuidToGuid(this.nativeService.UUID); } }
        public bool IsPrimary { get { return nativeService.Primary; } }

        public string Name
        {
            get
            {
                if (name == null)
                    name = KnownServices.Lookup(this.ID).Name;
                return name;
            }
        }

        public static Guid ServiceUuidToGuid(CBUUID uuid)
        {
            //this sometimes returns only the significant bits, e.g.
            //180d or whatever. so we need to add the full string
            string id = uuid.ToString();
            if (id.Length == 4)
                id = "0000" + id + "-0000-1000-8000-00805f9b34fb";
            return Guid.ParseExact(id, "d");
        }

        public void DiscoverCharacteristics()
        {
            this.parentDevice.DiscoverCharacteristics(this.nativeService);
        }

        public ICharacteristic FindCharacteristic(KnownCharacteristic characteristic)
        {
            foreach (var item in this.nativeService.Characteristics)
            {
                if (string.Equals(item.UUID.ToString(), characteristic.ID.ToString()))
                {
                    return new Characteristic(item, parentDevice);
                }
            }
            return null;
        }

        public void OnCharacteristicsDiscovered()
        {
            CharacteristicsDiscovered(this, new EventArgs());
        }
    }
}