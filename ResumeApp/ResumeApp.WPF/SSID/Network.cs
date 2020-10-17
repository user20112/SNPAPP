using System;

namespace ResumeApp.WPF.SSID
{
    public class Network
    {
        private readonly INetwork network;

        internal Network(INetwork network)
        {
            this.network = network;
        }

        public NetworkCategory Category
        {
            get
            {
                return network.GetCategory();
            }
            set
            {
                network.SetCategory(value);
            }
        }

        public DateTime ConnectedTime
        {
            get
            {
                network.GetTimeCreatedAndConnected(out _, out _, out uint low, out uint high);
                long time = high;
                time <<= 32;
                time |= low;
                return DateTime.FromFileTimeUtc(time);
            }
        }

        public NetworkConnectionCollection Connections
        {
            get
            {
                return new NetworkConnectionCollection(network.GetNetworkConnections());
            }
        }

        public ConnectivityStates Connectivity
        {
            get
            {
                return network.GetConnectivity();
            }
        }

        public DateTime CreatedTime
        {
            get
            {
                network.GetTimeCreatedAndConnected(out uint low, out uint high, out _, out _);
                long time = high;
                time <<= 32;
                time |= low;
                return DateTime.FromFileTimeUtc(time);
            }
        }

        public string Description
        {
            get
            {
                return network.GetDescription();
            }
            set
            {
                network.SetDescription(value);
            }
        }

        public DomainType DomainType
        {
            get
            {
                return network.GetDomainType();
            }
        }

        public bool IsConnected
        {
            get
            {
                return network.IsConnected;
            }
        }

        public bool IsConnectedToInternet
        {
            get
            {
                return network.IsConnectedToInternet;
            }
        }

        public string Name
        {
            get
            {
                return network.GetName();
            }
            set
            {
                network.SetName(value);
            }
        }

        public Guid NetworkId
        {
            get
            {
                return network.GetNetworkId();
            }
        }
    }
}