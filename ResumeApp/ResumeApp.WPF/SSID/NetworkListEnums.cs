using System;

namespace ResumeApp.WPF.SSID
{
    [Flags]
    public enum ConnectivityStates
    {
        None = 0,
        IPv4Internet = 0x40,
        IPv4LocalNetwork = 0x20,
        IPv4NoTraffic = 1,
        IPv4Subnet = 0x10,
        IPv6Internet = 0x400,
        IPv6LocalNetwork = 0x200,
        IPv6NoTraffic = 2,
        IPv6Subnet = 0x100
    }

    public enum DomainType
    {
        NonDomainNetwork = 0,
        DomainNetwork = 1,
        DomainAuthenticated = 2,
    }

    public enum NetworkCategory
    {
        Public,
        Private,
        Authenticated
    }

    [Flags]
    public enum NetworkConnectivityLevels
    {
        Connected = 1,
        Disconnected = 2,
        All = 3,
    }
}