using System;
using Toolbox.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utils;

namespace Runtime.Networking
{
    public class NetworkingController : NetworkBehaviourSingleton<NetworkingController>
    {
        [SerializeField, Range(1, 2)] private int requiredPlayers = 2;
        [SerializeField] private KeyCode forceStartKey = KeyCode.P;
        [SerializeField] private string targetScene;

#if (UNITY_EDITOR)
        [SerializeField] private UnityEditor.SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null) targetScene = sceneAsset.name;
        }
#endif

        public UnityEvent onPlayerJoin = new();
        public UnityEvent onPlayerLeave = new();

        private readonly NetworkVariable<bool> _isServerNurse = new();

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandlePlayerJoin;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandlePlayerDisconnect;
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(forceStartKey) &&
                (!NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
            {
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            }
        }

        private void HandlePlayerJoin(ulong id)
        {
            onPlayerJoin?.Invoke();

            if (!NetworkManager.Singleton.IsServer) return;

            if (NetworkManager.Singleton.ConnectedClients.Count == requiredPlayers)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            }
        }

        private void HandlePlayerDisconnect(ulong id)
        {
            onPlayerLeave?.Invoke();
        }

        public NetworkVariable<bool> IsServerNurse
        {
            get => _isServerNurse;
        }
    }
}