using System;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking.Discovery
{
    /// <summary>
    /// Our custom implementation of the NetworkDiscovery component
    /// overrides the receive methodes which our custom implementation.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkDiscoveryHandler : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
    {
        private NetworkManager _networkManager;
        private string _serverName = "Default-serverName";
        
        public UnityEvent<IPEndPoint, DiscoveryResponseData> onServerFound;

        /// <summary>
        /// The getter and setter for the ServerName, this is send to the client so it knows which server we are.
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set => _serverName = value;
        }
        
        /// <summary>
        /// Fetches the networkManager.
        /// </summary>
        public void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }
    
        /// <summary>
        /// handles the client's broadcast and sends a response back to the client with our information.
        /// </summary>
        /// <param name="sender">The sender from which we received the initial broadcast</param>
        /// <param name="broadCast">The broadcast data we received from the client</param>
        /// <param name="response">The response object we send back to the client.</param>
        /// <returns>whether or not we want to send data back.</returns>
        protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
        {
            response = new DiscoveryResponseData()
            {
                ServerName = ServerName,
                Port = ((UnityTransport) _networkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
            };
            return true;
        }

        /// <summary>
        /// Called when we receive a message from a server
        /// </summary>
        /// <param name="sender">The server from which we received this message</param>
        /// <param name="response">The response data we got back from our broadcast.</param>
        protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
        {
            Debug.Log("[NetCodeDiscovery] Found a new server \n " + response + " \n \n " + sender.Address);
            onServerFound.Invoke(sender, response);
        }
    }
}