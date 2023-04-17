using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.Events;

namespace Runtime.Networking.NetworkEvent
{
    [Serializable]
    public class NetworkEvent : IDisposable
    {
        private NetworkEventPermission _invokePermission;
        private NetworkObject _ownerObject;
        private string _eventNameID;

        private bool _isInitialized = false;

        public UnityEvent called = new();
        public UnityEvent calledClient = new();
        public UnityEvent calledServer = new();

        public NetworkEvent(NetworkEventPermission invokePermission = NetworkEventPermission.Server)
        {
            _invokePermission = invokePermission;
        }

        public void Initialize(NetworkObject networkObject)
        {
            if (_isInitialized) return;

            _isInitialized = true;

            _eventNameID = networkObject.name + networkObject.NetworkObjectId;
            _ownerObject = networkObject;


            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(_eventNameID, ReceiveMessage);
        }

        public void Dispose()
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(_eventNameID);
        }

        private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
        {
            called?.Invoke();
            if (NetworkManager.Singleton.IsClient) calledClient?.Invoke();
            if (NetworkManager.Singleton.IsServer) calledServer?.Invoke();
        }

        public void Invoke()
        {
            if (!CanInvoke()) return;
            
            var writer = new FastBufferWriter(0, Allocator.Temp);
            var customMessagingManager = NetworkManager.Singleton.CustomMessagingManager;
            using (writer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    customMessagingManager.SendNamedMessageToAll(_eventNameID, writer);
                }
                else
                {
                    customMessagingManager.SendNamedMessage(_eventNameID, NetworkManager.ServerClientId, writer);
                }
            }
        }

        private bool CanInvoke()
        {
            return _invokePermission switch
            {
                NetworkEventPermission.Server => NetworkManager.Singleton.IsServer,
                NetworkEventPermission.Owner => _ownerObject.IsOwner,
                NetworkEventPermission.Everyone => true,
                _ => false
            };
        }
    }
}