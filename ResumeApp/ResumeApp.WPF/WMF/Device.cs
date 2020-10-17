using System;

namespace ResumeApp.WPF.Classes.WMF
{
    /// <summary>
    /// A WMF device for the  Application. Basically just a correlation
    /// between the device symbolic name and the friendly name
    ///
    /// </summary>
    public class MFDevice : ObjBase, IComparable
    {
        private Guid deviceType = Guid.Empty;
        private string friendlyName = "";
        private string symbolicName = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="friendlyNameIn">the friendlyName</param>
        /// <param name="symbolicLinkNameIn">the symbolicLinkName</param>
        /// <param name="deviceTypeIn">the device type</param>
        public MFDevice(string friendlyNameIn, string symbolicLinkNameIn, Guid deviceTypeIn)
        {
            FriendlyName = friendlyNameIn;
            SymbolicName = symbolicLinkNameIn;
            DeviceType = deviceTypeIn;
        }

        /// <summary>
        /// Gets/Sets the Guid of the device type.
        /// </summary>
        public Guid DeviceType
        {
            get
            {
                return deviceType;
            }
            set
            {
                deviceType = value;
            }
        }

        /// <summary>
        /// Gets/Sets the friendly name. Never gets/sets null. Will return empty.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                // we never return null
                if (friendlyName == null) friendlyName = "";
                return friendlyName;
            }
            set
            {
                friendlyName = value;
                if (friendlyName == null) friendlyName = "";
            }
        }

        /// <summary>
        /// Gets/Sets the symbolic name. Never gets/sets null. Will return empty.
        /// </summary>
        public string SymbolicName
        {
            get
            {
                // we never return null
                if (symbolicName == null) symbolicName = "";
                return symbolicName;
            }
            set
            {
                symbolicName = value;
                if (symbolicName == null) symbolicName = "";
            }
        }

        /// <summary>
        /// IComparable implementation
        /// </summary>
        /// <param name="obj">Our device to compare to</param>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if ((obj is MFDevice) == false) return 2;
            if (SymbolicName != (obj as MFDevice).SymbolicName) return 20;
            if (FriendlyName != (obj as MFDevice).FriendlyName) return 30;
            return 0;
        }

        /// <summary>
        /// The ToString() override.
        /// </summary>
        public override string ToString()
        {
            return FriendlyName;
        }
    }
}