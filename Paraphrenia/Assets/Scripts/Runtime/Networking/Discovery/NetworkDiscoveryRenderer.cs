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
        
        private NetworkDiscoveryHandler _networkDiscovery;
        private readonly Dictionary<IPAddress, DiscoveryResponseData> _discoveredServers = new();



        private void Awake()
        {
            _networkDiscovery = FindObjectOfType<NetworkDiscoveryHandler>();
            _networkDiscovery.onServerFound.AddListener(HandleNetServerFound);
            createServerButton.onClick.AddListener(StartServer);
            refreshListButton.onClick.AddListener(RefreshList);
        }

        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            _networkDiscovery.ServerName = serverNameInputField.text;
            _networkDiscovery.StartServer();
        }
        
        
        void HandleNetServerFound(IPEndPoint sender, DiscoveryResponseData response)
        {
            Debug.Log("[NetCodeDiscovery] Found new server and updating HUD");
            _discoveredServers[sender.Address] = response;
            RenderServerItem(response);
        }

        private void RefreshList()
        {
            RemoveOldServers();
            _discoveredServers.Clear();
            _networkDiscovery.StartClient();
            _networkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
        }

        private void RemoveOldServers()
        {
            Transform[] transforms = serverListContainer.GetComponentsInChildren<Transform>();
            for (var i = transforms.Length - 1; i >= 0; i--)
            {
                if (transforms[i] == serverListContainer.transform) continue;
                Destroy(transforms[i]);
            }
        }

        private void RenderServerItem(DiscoveryResponseData data)
        {
            ServerItemRenderer serverPreview = Instantiate(serverItemRendererPrefab, Vector3.back, Quaternion.identity,
                serverListContainer.transform);
            
            serverPreview.SetName(data.ServerName);
        }
    }
}
