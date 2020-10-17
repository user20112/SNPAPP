using System;

namespace ResumeApp.WPF.SSID
{
    public static class NetworkListManager
    {
        private static readonly NetworkListManagerClass manager = new NetworkListManagerClass();

        public static ConnectivityStates Connectivity
        {
            get
            {
                return manager.GetConnectivity();
            }
        }

        public static bool IsConnected
        {
            get
            {
                return manager.IsConnected;
            }
        }

        public static bool IsConnectedToInternet
        {
            get
            {
                return manager.IsConnectedToInternet;
            }
        }

        public static Network GetNetwork(Guid networkId)
        {
            return new Network(manager.GetNetwork(networkId));
        }

        public static NetworkConnection GetNetworkConnection(Guid networkConnectionId)
        {
            return new NetworkConnection(manager.GetNetworkConnection(networkConnectionId));
        }

        public static NetworkConnectionCollection GetNetworkConnections()
        {
            return new NetworkConnectionCollection(manager.GetNetworkConnections());
        }

        public static NetworkCollection GetNetworks(NetworkConnectivityLevels level)
        {
            return new NetworkCollection(manager.GetNetworks(level));
        }
    }
}