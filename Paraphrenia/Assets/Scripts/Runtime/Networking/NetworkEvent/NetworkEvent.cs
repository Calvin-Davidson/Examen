using System;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon.StructWrapping;
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

        private bool _isInitialized;

        public UnityEvent called = new();
        public UnityEvent calledClient = new();
        public UnityEvent calledServer = new();

        public NetworkEvent(NetworkEventPermission invokePermission = NetworkEventPermission.Server)
        {
            _invokePermission = invokePermission;
        }

        public void Initialize(NetworkBehaviour behaviour)
        {
            if (_isInitialized) return;

            _isInitialized = true;

            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.Public;

            var events = behaviour.GetType()
                .GetFields(bindingFlags).Where(field => field.GetValue(behaviour).IsType<NetworkEvent>())
                .Select(field => field.GetValue(behaviour) as NetworkEvent)
                .ToList();


            _eventNameID = behaviour.NetworkObject.name + behaviour.NetworkObject.NetworkObjectId + events.IndexOf(this);
            _ownerObject = behaviour.NetworkObject;


            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(_eventNameID, ReceiveMessage);
        }

        public void Dispose()
        {
            if (NetworkManager.Singleton == null) return;
            if (NetworkManager.Singleton.CustomMessagingManager == null) return;
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
            if (!_isInitialized) throw new Exception("Cannot invoke a networkEvent that is not initialized");

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