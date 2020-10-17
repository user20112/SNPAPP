using System;

namespace ResumeApp.WPF.SSID
{
    public class NetworkConnection
    {
        private readonly INetworkConnection networkConnection;

        internal NetworkConnection(INetworkConnection networkConnection)
        {
            this.networkConnection = networkConnection;
        }

        public Guid AdapterId
        {
            get
            {
                return networkConnection.GetAdapterId();
            }
        }

        public Guid ConnectionId
        {
            get
            {
                return networkConnection.GetConnectionId();
            }
        }

        public ConnectivityStates Connectivity
        {
            get
            {
                return networkConnection.GetConnectivity();
            }
        }

        public DomainType DomainType
        {
            get
            {
                return networkConnection.GetDomainType();
            }
        }

        public bool IsConnected
        {
            get
            {
                return networkConnection.IsConnected;
            }
        }

        public bool IsConnectedToInternet
        {
            get
            {
                return networkConnection.IsConnectedToInternet;
            }
        }

        public Network Network
        {
            get
            {
                return new Network(networkConnection.GetNetwork());
            }
        }
    }
}