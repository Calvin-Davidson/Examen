using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking.NetworkEvent
{
    public class NetworkEvent : IDisposable
    {
        private NetworkEventPermission _listenPermission;
        private NetworkEventPermission _invokePermission;
        private NetworkObject _ownerObject;
        private string _eventNameID;

        private bool _isInitialized = false;

        public NetworkEvent(NetworkEventPermission listenPermission = NetworkEventPermission.Everyone,
            NetworkEventPermission invokePermission = NetworkEventPermission.Server)
        {
            _listenPermission = listenPermission;
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
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log($"Server received from client ({senderId})");
            }
            else
            {
                Debug.Log($"Client received from the server.");
            }
        }

        public void SendMessage()
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
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
    }
}