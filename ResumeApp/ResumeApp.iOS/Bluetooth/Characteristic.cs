using CoreBluetooth;
using Foundation;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NativeFeatures.IOS
{
    public class Characteristic : ICharacteristic
    {
        protected IList<IDescriptor> _descriptors;
        protected CBCharacteristic nativeCharacteristic;
        private readonly CBPeripheral parentDevice;

        public Characteristic(CBCharacteristic nativeCharacteristic, CBPeripheral parentDevice)
        {
            this.nativeCharacteristic = nativeCharacteristic;
            this.parentDevice = parentDevice;
        }

        public event EventHandler<CharacteristicReadEventArgs> ValueUpdated = delegate { };

        public bool CanRead { get { return (this.Properties & CharacteristicPropertyType.Read) != 0; } }
        public bool CanUpdate { get { return (this.Properties & CharacteristicPropertyType.Notify) != 0; } }
        public bool CanWrite { get { return (this.Properties & (CharacteristicPropertyType.WriteWithoutResponse | CharacteristicPropertyType.AppleWriteWithoutResponse)) != 0; } }

        public IList<IDescriptor> Descriptors
        {
            get
            {
                // if we haven't converted them to our xplat objects
                if (this._descriptors != null)
                {
                    this._descriptors = new List<IDescriptor>();
                    // convert the private list of them to the xplat ones
                    foreach (var item in this.nativeCharacteristic.Descriptors)
                    {
                        this._descriptors.Add(new Descriptor(item));
                    }
                }
                return this._descriptors;
            }
        }

        public Guid ID
        {
            get { return CharacteristicUuidToGuid(this.nativeCharacteristic.UUID); }
        }

        public string Name
        {
            get { return KnownCharacteristics.Lookup(this.ID).Name; }
        }

        public object NativeCharacteristic
        {
            get
            {
                return this.nativeCharacteristic;
            }
        }

        public CharacteristicPropertyType Properties
        {
            get
            {
                return (CharacteristicPropertyType)(int)this.nativeCharacteristic.Properties;
            }
        }

        public string StringValue
        {
            get
            {
                if (this.Value == null)
                    return String.Empty;
                else
                {
                    var stringByes = this.Value;
                    var s1 = System.Text.Encoding.UTF8.GetString(stringByes);
                    return s1;
                }
            }
        }

        public string Uuid
        {
            get { return this.nativeCharacteristic.UUID.ToString(); }
        }

        public byte[] Value
        {
            get
            {
                if (nativeCharacteristic.Value == null)
                    return null;
                return this.nativeCharacteristic.Value.ToArray();
            }
        }

        //TODO: this is the exact same as ServiceUuid i think
        public static Guid CharacteristicUuidToGuid(CBUUID uuid)
        {
            //this sometimes returns only the significant bits, e.g.
            //180d or whatever. so we need to add the full string
            string id = uuid.ToString();
            if (id.Length == 4)
            {
                id = "0000" + id + "-0000-1000-8000-00805f9b34fb";
            }
            return Guid.ParseExact(id, "d");
        }

        public void Dispose()
        {
            StopUpdates();
        }

        public Task<ICharacteristic> ReadAsync()
        {
            var tcs = new TaskCompletionSource<ICharacteristic>();
            if (!CanRead)
            {
                throw new InvalidOperationException("Characteristic does not support READ");
            }
            parentDevice.UpdatedCharacterteristicValue += null;
            Console.WriteLine(".....ReadAsync");
            parentDevice.ReadValue(nativeCharacteristic);
            return tcs.Task;
        }

        public void StartUpdates()
        {
            bool successful = false;
            if (CanUpdate)
            {
                Console.WriteLine("** Characteristic.RequestValue, PropertyType = Notify, requesting updates");
                parentDevice.UpdatedCharacterteristicValue += UpdatedNotify;
                parentDevice.SetNotifyValue(true, nativeCharacteristic);
                successful = true;
            }
            Console.WriteLine("** RequestValue, Succesful: " + successful.ToString());
        }

        public void StopUpdates()
        {
            if (CanUpdate)
            {
                parentDevice.SetNotifyValue(false, nativeCharacteristic);
                Console.WriteLine("** Characteristic.RequestValue, PropertyType = Notify, STOP updates");
            }
        }

        public void Write(byte[] data)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Characteristic does not support WRITE");
            }
            var nsdata = NSData.FromArray(data);
            var descriptor = (CBCharacteristic)nativeCharacteristic;
            var t = (Properties & CharacteristicPropertyType.AppleWriteWithoutResponse) != 0 ?
                CBCharacteristicWriteType.WithoutResponse :
                CBCharacteristicWriteType.WithResponse;
            parentDevice.WriteValue(nsdata, descriptor, t);
            return;
        }

        private void UpdatedNotify(object sender, CBCharacteristicEventArgs e)
        {
            this.ValueUpdated(this, new CharacteristicReadEventArgs()
            {
                Characteristic = new Characteristic(e.Characteristic, parentDevice)
            });
        }

        private void UpdatedRead(object sender, CBCharacteristicEventArgs e)
        {
            this.ValueUpdated(this, new CharacteristicReadEventArgs()
            {
                Characteristic = new Characteristic(e.Characteristic, parentDevice)
            });
            parentDevice.UpdatedCharacterteristicValue -= UpdatedRead;
        }
    }
}