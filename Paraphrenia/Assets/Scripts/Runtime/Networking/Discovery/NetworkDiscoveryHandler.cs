using System;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking.Discovery
{
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkDiscoveryHandler : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
    {
        private NetworkManager _networkManager;
        private string serverName = "Default-serverName";
        public UnityEvent<IPEndPoint, DiscoveryResponseData> onServerFound;


        public string ServerName
        {
            get => serverName;
            set => serverName = value;
        }
        
        public void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }
    
        protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
        {
            response = new DiscoveryResponseData()
            {
                ServerName = serverName,
                Port = ((UnityTransport) _networkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
            };
            return true;
        }

        protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
        {
            Debug.Log("[NetCodeDiscovery] Found a new server \n " + response + " \n \n " + sender.Address);
            onServerFound.Invoke(sender, response);
        }
    }
}