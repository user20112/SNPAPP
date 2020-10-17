using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using ResumeApp.UWP.Bluetooth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

[assembly: Xamarin.Forms.Dependency(typeof(Characteristic))]

namespace ResumeApp.UWP.Bluetooth
{
    public class Characteristic : ICharacteristic
    {
        protected GattCharacteristicWithValue gattCharacteristicWithValue;
        private IList<IDescriptor> _descriptors = null;

        public Characteristic(GattCharacteristicWithValue gattCharacteristicWithValue)
        {
            this.gattCharacteristicWithValue = gattCharacteristicWithValue;
        }

        public Characteristic(GattCharacteristic nativeCharacteristic)
        {
            this.gattCharacteristicWithValue = new GattCharacteristicWithValue(nativeCharacteristic);
        }

        public event EventHandler<CharacteristicReadEventArgs> ValueUpdated;

        public bool CanRead
        {
            get
            {
                if (CheckGattProperty(GattCharacteristicProperties.Read))
                    return true;
                return false;
            }
        }

        public bool CanUpdate
        {
            get
            {
                if (CheckGattProperty(GattCharacteristicProperties.Notify))
                    return true;
                return false;
            }
        }

        public bool CanWrite
        {
            get
            {
                if (CheckGattProperty(GattCharacteristicProperties.Write) || CheckGattProperty(GattCharacteristicProperties.WriteWithoutResponse))
                    return true;
                return false;
            }
        }

        public IList<IDescriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    this._descriptors = new List<IDescriptor>();
                    foreach (KnownDescriptor kd in KnownDescriptors.GetDescriptors())
                    {
                        var task = gattCharacteristicWithValue.NativeCharacteristic.GetDescriptorsForUuidAsync(kd.ID);
                        while (task.Status == Windows.Foundation.AsyncStatus.Started)
                            Thread.Sleep(50);
                        GattDescriptorsResult temp = task.GetResults();
                        _descriptors.Add(new Descriptor(temp.Descriptors[0]));
                    }
                }
                return _descriptors;
            }
        }

        public Guid ID
        {
            get { return gattCharacteristicWithValue.ID; }
        }

        public string Name
        {
            get { return this.ID.ToString(); }
        }

        public object NativeCharacteristic
        {
            get { return gattCharacteristicWithValue; }
        }

        public CharacteristicPropertyType Properties
        {
            get { return (CharacteristicPropertyType)(int)this.gattCharacteristicWithValue.NativeCharacteristic.CharacteristicProperties; }
        }

        public string StringValue
        {
            get { return Encoding.ASCII.GetString(gattCharacteristicWithValue.Value); }
        }

        public string Uuid
        {
            get { return gattCharacteristicWithValue.Uuid; }
        }

        public byte[] Value
        {
            get { return gattCharacteristicWithValue.Value; }
        }

        public bool CheckGattProperty(GattCharacteristicProperties gattProperty)
        {
            if (((int)gattCharacteristicWithValue.NativeCharacteristic.CharacteristicProperties & (int)gattProperty) != 0)
                return true;
            return false;
        }

        public void Dispose()
        {
            StopUpdates();
        }

        public async Task<ICharacteristic> ReadAsync()
        {
            GattCharacteristicWithValue val = new GattCharacteristicWithValue
            {
                NativeCharacteristic = this.gattCharacteristicWithValue.NativeCharacteristic
            };
            if (!CanRead)
            {
                throw new InvalidOperationException("Characteristic does not support READ");
            }
            try
            {
                GattReadResult readResult = await this.gattCharacteristicWithValue.NativeCharacteristic.ReadValueAsync();
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    val.Value = new byte[readResult.Value.Length];
                    DataReader.FromBuffer(readResult.Value).ReadBytes(val.Value);
                }
            }
            catch { }
            return new Characteristic(val);
        }

        public async void StartUpdates()
        {
            if (CanRead)
            {
                Debug.WriteLine("** Characteristic.RequestValue, PropertyType = Read, requesting read");
                try
                {
                    var status = await gattCharacteristicWithValue.NativeCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Characteric:StartUpdates failed: " + ex.Message);
                }
                if ((gattCharacteristicWithValue.NativeCharacteristic.CharacteristicProperties & GattCharacteristicProperties.Notify) == GattCharacteristicProperties.Notify)
                    gattCharacteristicWithValue.NativeCharacteristic.ValueChanged += ValueChanged;
            }
            else if (CanUpdate)
            {
                Debug.WriteLine("** Characteristic.RequestValue, PropertyType = Notify, requesting updates");
                if (gattCharacteristicWithValue.NativeCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Notify)
                    gattCharacteristicWithValue.NativeCharacteristic.ValueChanged += ValueChanged;
            }
        }

        public void StopUpdates()
        {
            var task = gattCharacteristicWithValue.NativeCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            while (task.Status == Windows.Foundation.AsyncStatus.Started)
                Thread.Sleep(50);
        }

        public async void Write(byte[] data)
        {
            Debug.WriteLine("Write received:" + Encoding.ASCII.GetString(data));
            var dataWriter = new DataWriter();
            dataWriter.WriteBytes(data);
            var buffer = dataWriter.DetachBuffer();
            try
            {
                var status = await gattCharacteristicWithValue.NativeCharacteristic.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
                if (status == GattCommunicationStatus.Success)
                    Debug.WriteLine("Write successful");
                else
                    Debug.WriteLine("Write unsuccessful");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Write unsuccessful " + ex.Message);
            }
        }

        private void ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var count = args.CharacteristicValue.Length;
            byte[] buffer = new byte[count];
            var data = String.Empty;
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(buffer);
            gattCharacteristicWithValue.Value = buffer;
            ValueUpdated?.Invoke(this, new CharacteristicReadEventArgs()
            {
                Characteristic = this,
            });
        }
    }

    public class GattCharacteristicWithValue
    {
        public GattCharacteristicWithValue()
        {
        }

        public GattCharacteristicWithValue(GattCharacteristic gattCharacteristic)
        {
            NativeCharacteristic = gattCharacteristic;
        }

        public Guid ID { get { return NativeCharacteristic.Uuid; } }
        public GattCharacteristic NativeCharacteristic { get; set; }
        public string Uuid { get { return NativeCharacteristic.Uuid.ToString(); } }
        public byte[] Value { get; set; }
    }
}