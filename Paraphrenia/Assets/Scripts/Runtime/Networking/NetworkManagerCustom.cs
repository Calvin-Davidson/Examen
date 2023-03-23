using Unity.Netcode;
using UnityEngine;

namespace Runtime.Networking
{
    public class NetworkManagerCustom : MonoBehaviour
    {
        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback+=HandleClientDisconnected;
        }

        private void HandleClientDisconnected(ulong obj)
        {
            Debug.Log("Disconnected: " + obj);
        }
        
    }
}
