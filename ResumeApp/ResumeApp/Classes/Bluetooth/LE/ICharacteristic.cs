using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public interface ICharacteristic : IDisposable
    {
        // events
        event EventHandler<CharacteristicReadEventArgs> ValueUpdated;

        bool CanRead { get; }
        bool CanUpdate { get; }
        bool CanWrite { get; }
        IList<IDescriptor> Descriptors { get; }

        // properties
        Guid ID { get; }

        string Name { get; }
        object NativeCharacteristic { get; }
        CharacteristicPropertyType Properties { get; }
        string StringValue { get; }
        string Uuid { get; }
        byte[] Value { get; }

        // methods
        //		void EnumerateDescriptors ();
        Task<ICharacteristic> ReadAsync();

        void StartUpdates();

        void StopUpdates();

        void Write(byte[] data);
    }
}