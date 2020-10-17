using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.UWP.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

[assembly: Xamarin.Forms.Dependency(typeof(Service))]

namespace ResumeApp.UWP.Bluetooth
{
    public class Service : IService
    {
        protected IList<ICharacteristic> _characteristics;
        protected Guid id = Guid.Empty;
        protected string name = null;
        protected GattDeviceService nativeService;

        public Service(GattDeviceService nativeService)
        {
            this.nativeService = nativeService;
        }

        public event EventHandler CharacteristicsDiscovered;

        public IList<ICharacteristic> Characteristics
        {
            get
            {
                if (_characteristics == null)
                {
                    this._characteristics = new List<ICharacteristic>();
                    var task = nativeService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    while (task.Status == Windows.Foundation.AsyncStatus.Started)
                        Thread.Sleep(50);
                    if (task.Status == Windows.Foundation.AsyncStatus.Completed)
                    {
                        var result = task.GetResults();
                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            this._characteristics = new List<ICharacteristic>();
                            foreach (GattCharacteristic characteristic in result.Characteristics)
                                this._characteristics.Add(new Characteristic(characteristic));
                        }
                        else
                            this._characteristics = new List<ICharacteristic>();
                    }
                    else
                        this._characteristics = new List<ICharacteristic>();
                }
                return _characteristics;
            }
        }

        public Guid ID
        {
            get
            {
                if (id == Guid.Empty)
                    id = ExtractGuid(this.nativeService.DeviceId);
                return id;
            }
        }

        public bool IsPrimary
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                if (this.name == null)
                    this.name = this.ID.ToString();
                return this.name;
            }
        }

        public void DiscoverCharacteristics()
        {
            if (Characteristics != null)
            {
                this.CharacteristicsDiscovered(this, new EventArgs());
            }
        }

        public ICharacteristic FindCharacteristic(KnownCharacteristic characteristic)
        {
            var c = this.nativeService.GetCharacteristics(characteristic.ID).FirstOrDefault();
            return new Characteristic(c);
        }

        private Guid ExtractGuid(string id)
        {
            int start = id.IndexOf('{') + 1;
            var guid = id.Substring(start, 36);
            return Guid.Parse(guid);
        }
    }
}