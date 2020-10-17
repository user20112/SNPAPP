using Android.Bluetooth;
using ResumeApp.Bluetooth.LE;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NativeFeatures.Droid
{
    public class Characteristic : ICharacteristic
    {
        protected IList<IDescriptor> _descriptors;
        protected BluetoothGatt _gatt;
        protected GattCallback _gattCallback;
        protected BluetoothGattCharacteristic _nativeCharacteristic;

        public Characteristic(BluetoothGattCharacteristic nativeCharacteristic, BluetoothGatt gatt, GattCallback gattCallback)
        {
            _nativeCharacteristic = nativeCharacteristic;
            _gatt = gatt;
            _gattCallback = gattCallback;
            if (_gattCallback != null)
            {
                _gattCallback.CharacteristicValueUpdated += ValueUpdatedFunction;
            }
        }

        ~Characteristic()
        {
            _gattCallback.CharacteristicValueUpdated -= ValueUpdatedFunction;
        }

        public event EventHandler<CharacteristicReadEventArgs> ValueUpdated = delegate { };

        public bool CanRead { get { return (this.Properties & CharacteristicPropertyType.Read) != 0; } }
        public bool CanUpdate { get { return (this.Properties & CharacteristicPropertyType.Notify) != 0; } }
        public bool CanWrite { get { return (this.Properties & CharacteristicPropertyType.WriteWithoutResponse | CharacteristicPropertyType.AppleWriteWithoutResponse) != 0; } }

        public IList<IDescriptor> Descriptors
        {
            get
            {
                if (this._descriptors == null)
                {
                    this._descriptors = new List<IDescriptor>();
                    foreach (var item in this._nativeCharacteristic.Descriptors)
                    {
                        this._descriptors.Add(new Descriptor(item));
                    }
                }
                return this._descriptors;
            }
        }

        public Guid ID
        {
            get { return Guid.Parse(this._nativeCharacteristic.Uuid.ToString()); }
        }

        public string Name
        {
            get { return KnownCharacteristics.Lookup(this.ID).Name; }
        }

        public object NativeCharacteristic
        {
            get
            {
                return this._nativeCharacteristic;
            }
        }

        public CharacteristicPropertyType Properties
        {
            get
            {
                return (CharacteristicPropertyType)(int)this._nativeCharacteristic.Properties;
            }
        }

        public string StringValue
        {
            get
            {
                if (this.Value == null)
                    return String.Empty;
                else
                    return System.Text.Encoding.UTF8.GetString(this.Value);
            }
        }

        public string Uuid
        {
            get { return this._nativeCharacteristic.Uuid.ToString(); }
        }

        public byte[] Value
        {
            get { return this._nativeCharacteristic.GetValue(); }
        }

        public void Dispose()
        {
            try
            {
                StopUpdates();
                _nativeCharacteristic.Dispose();
                _gatt.Dispose();
            }
            catch
            {
            }
        }

        public Task<ICharacteristic> ReadAsync()
        {
            var tcs = new TaskCompletionSource<ICharacteristic>();
            if (!CanRead)
            {
                throw new InvalidOperationException("Characteristic does not support READ");
            }
            if (this._gattCallback != null)
            {
                // wire up the characteristic value updating on the gattcallback
                this._gattCallback.CharacteristicValueUpdated += null;
            }
            this._gatt.ReadCharacteristic(this._nativeCharacteristic);
            return tcs.Task;
        }

        //public void StartUpdates()
        //{
        //    Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            if (!_gatt.SetCharacteristicNotification(_nativeCharacteristic, true))
        //            {
        //                // In order to subscribe to notifications on a given characteristic, you must first set the Notifications Enabled bit
        //                // in its Client Characteristic Configuration Descriptor. See https://developer.bluetooth.org/gatt/descriptors/Pages/DescriptorsHomePage.aspx and
        //                // https://developer.bluetooth.org/gatt/descriptors/Pages/DescriptorViewer.aspx?u=org.bluetooth.descriptor.gatt.client_characteristic_configuration.xml
        //                // for details.

        //                if (_nativeCharacteristic.Descriptors.Count > 0)
        //                {
        //                    var descriptors = _nativeCharacteristic.Descriptors;
        //                    var descriptor = descriptors[0];
        //                    if (descriptor != null && Properties.HasFlag(CharacteristicPropertyType.Indicate))
        //                    {
        //                        if (!descriptor.SetValue(BluetoothGattDescriptor.EnableIndicationValue.ToArray()))
        //                        {
        //                        }
        //                        if (!_gatt.WriteDescriptor(descriptor))
        //                        {
        //                        }
        //                    }
        //                    if (descriptor != null && Properties.HasFlag(CharacteristicPropertyType.Notify))
        //                    {
        //                        if (!descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray()))
        //                        {
        //                        }
        //                        if (!_gatt.WriteDescriptor(descriptor))
        //                        {
        //                        }
        //                    }
        //                    break;
        //                }
        //                else
        //                {
        //                }
        //            }
        //            else
        //            {
        //            }
        //            Thread.Sleep(1000);
        //        }
        //    });
        //}

        public void StartUpdates()
        {
            Task.Run(() =>
            {//on andorid this NEEDS to be in a different thread. when you await each command it stops it from responding and unblocking.
                while (!_gatt.ReadCharacteristic(_nativeCharacteristic))
                {
                    Thread.Sleep(100);
                }
                while (!_gatt.SetCharacteristicNotification(_nativeCharacteristic, true))
                {
                    Thread.Sleep(100);
                }
                // [TO20131211@1634] It seems that setting the notification above isn't enough. You have to set the NOTIFY
                // descriptor as well, otherwise the receiver will never get the updates. I just grabbed the first (and only)
                // descriptor that is associated with the characteristic, which is the NOTIFY descriptor. This seems like a really
                // odd way to do things to me, but I'm a Bluetooth newbie. Google has a example here (but no real explaination as
                // to what is going on):
                if (_nativeCharacteristic.Descriptors.Count > 0)
                {
                    Thread.Sleep(100);
                    BluetoothGattDescriptor descriptor = _nativeCharacteristic.Descriptors[0];
                    while (!descriptor.SetValue(BluetoothGattDescriptor.EnableIndicationValue.ToArray()))
                    {
                        Console.WriteLine("Descriptor Setvalue loop");
                        Thread.Sleep(100);
                    }
                    while (!_gatt.WriteDescriptor(descriptor))
                    {
                        Console.WriteLine(" Write Descriptor loop");
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(100);
                    while (!descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray()))
                    {
                        Console.WriteLine("Descriptor Setvalue loop");
                        Thread.Sleep(100);
                    }
                    while (!_gatt.WriteDescriptor(descriptor))
                    {
                        Console.WriteLine(" Write Descriptor loop");
                        Thread.Sleep(100);
                    }
                }
            });
        }

        public void StopUpdates()
        {
            if (CanUpdate)
            {
                this._gatt.SetCharacteristicNotification(this._nativeCharacteristic, false);
            }
        }

        public void Write(byte[] data)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Characteristic does not support WRITE");
            }
            _nativeCharacteristic.WriteType = GattWriteType.Default;
            _nativeCharacteristic.SetValue(data);
            if (!_gatt.WriteCharacteristic(_nativeCharacteristic))
            {
                Thread.Sleep(100);
                if (!_gatt.WriteCharacteristic(_nativeCharacteristic))
                {
                    Thread.Sleep(100);
                    _gatt.WriteCharacteristic(_nativeCharacteristic);
                }
            }
        }

        private void ValueUpdatedFunction(object sender, CharacteristicReadEventArgs e)
        {
            ValueUpdated(this, e);
        }
    }
}