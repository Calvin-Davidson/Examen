using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Runtime.Networking
{
    public class NetworkingController : MonoBehaviour
    {
        [SerializeField, Range(1, 2)] private int requiredPlayers = 2;
        [SerializeField] private KeyCode forceStartKey = KeyCode.P;
        
        public UnityEvent onPlayerJoin = new();
        public UnityEvent onPlayerLeave = new();
        
        
        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandlePlayerJoin;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandlePlayerDisconnect;
        }

        private void Update()
        {
            if (Input.GetKeyDown(forceStartKey) && (!NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
            {
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.LoadScene("PlayerMovement", LoadSceneMode.Single);
            }
        }

        private void HandlePlayerJoin(ulong id)
        {
            onPlayerJoin?.Invoke();

            if (!NetworkManager.Singleton.IsServer) return;
            
            if (NetworkManager.Singleton.ConnectedClients.Count == requiredPlayers)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("PlayerMovement", LoadSceneMode.Single);
            }
        }

        private void HandlePlayerDisconnect(ulong id)
        {
            onPlayerLeave?.Invoke();
        }
    }
}
