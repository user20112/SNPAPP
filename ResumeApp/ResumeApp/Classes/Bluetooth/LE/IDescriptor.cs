using System;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public interface IDescriptor
    {
        Guid ID { get; }
        string Name { get; }
        object NativeDescriptor { get; }
    }
}