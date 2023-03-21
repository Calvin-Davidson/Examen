using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Networking.Discovery
{
    public class NetworkDiscoveryRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject serverListContainer;
        [SerializeField] private ServerItemRenderer serverItemRendererPrefab;
        [SerializeField] private Button createServerButton;
        [SerializeField] private Button refreshListButton;
        [SerializeField] private TMP_InputField serverNameInputField;
        
        /// <summary>
        /// The NetworkDiscoveryHandler to which we subscribe.
        /// </summary>
        private NetworkDiscoveryHandler _networkDiscovery;
        
        /// <summary>
        /// A keyValue set of the servers from which we received responses.
        /// </summary>
        private readonly Dictionary<IPAddress, DiscoveryResponseData> _discoveredServers = new();


        /// <summary>
        /// Fetches the required components and subscribes to there events.
        /// </summary>
        private void Awake()
        {
            _networkDiscovery = FindObjectOfType<NetworkDiscoveryHandler>();
            _networkDiscovery.onServerFound.AddListener(HandleNetServerFound);
            createServerButton.onClick.AddListener(StartServer);
            refreshListButton.onClick.AddListener(RefreshList);
        }

        /// <summary>
        /// Start's the NetCode server and start's listening for client broadcasts. 
        /// </summary>
        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            _networkDiscovery.ServerName = serverNameInputField.text;
            _networkDiscovery.StartServer();
        }
        
        /// <summary>
        /// Called when we find a new server.
        /// </summary>
        /// <param name="sender">The server from which we received this response</param>
        /// <param name="response">the response data object we received from the server</param>
        void HandleNetServerFound(IPEndPoint sender, DiscoveryResponseData response)
        {
            Debug.Log("[NetCodeDiscovery] Found new server and updating HUD");
            if (_discoveredServers.ContainsKey(sender.Address))
            {
                Debug.Log("[NetCodeDiscovery] Received a response that's already in the list, Ignoring it.");
                return;
            }
            _discoveredServers[sender.Address] = response;
            RenderServerItem(response);
        }

        /// <summary>
        /// Clear's the list and resends the broadcast message to all servers.
        /// </summary>
        private void RefreshList()
        {
            RemoveOldServers();
            _discoveredServers.Clear();
            _networkDiscovery.StartClient();
            _networkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
        }

        /// <summary>
        /// Remove's the old servers from the list by destroying the game-objects.
        /// </summary>
        private void RemoveOldServers()
        {
            Transform[] transforms = serverListContainer.GetComponentsInChildren<Transform>();
            for (var i = transforms.Length - 1; i >= 0; i--)
            {
                if (transforms[i] == serverListContainer.transform) continue;
                Destroy(transforms[i].gameObject);
            }
        }

        /// <summary>
        /// Adds the ResponseData server to the list.
        /// </summary>
        /// <param name="data">The data we received from the server, from which we get the serverName</param>
        private void RenderServerItem(DiscoveryResponseData data)
        {
            ServerItemRenderer serverPreview = Instantiate(serverItemRendererPrefab, Vector3.back, Quaternion.identity,
                serverListContainer.transform);
            
            serverPreview.SetName(data.ServerName);
        }
    }
}
