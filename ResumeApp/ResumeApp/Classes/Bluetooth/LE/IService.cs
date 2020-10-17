using System;
using System.Collections.Generic;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public interface IService
    {
        event EventHandler CharacteristicsDiscovered;

        IList<ICharacteristic> Characteristics { get; }
        Guid ID { get; }
        bool IsPrimary { get; }
        String Name { get; }

        //IDictionary<Guid, ICharacteristic> Characteristics { get; }
        void DiscoverCharacteristics();

        // iOS only?
        ICharacteristic FindCharacteristic(KnownCharacteristic characteristic);
    }
}