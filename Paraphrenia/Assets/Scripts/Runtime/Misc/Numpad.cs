using System;
using Runtime.Interaction;
using Runtime.Networking.NetworkEvent;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using NetworkEvent = Runtime.Networking.NetworkEvent.NetworkEvent;

namespace Runtime.Misc
{
    public class Numpad : NetworkBehaviour
    {
        [SerializeField, Tooltip("Should be added in order from 0 to infinite")]
        private NetworkedInteractable[] numpadPads;

        [SerializeField] private string code;

        [SerializeField] private bool autoSubmit = false;
        
        private string _insertedCode;

        public NetworkEvent onCodeFailed = new(NetworkEventPermission.Everyone);
        public NetworkEvent onCodeSuccess = new(NetworkEventPermission.Everyone);
        public NetworkEvent onPadPressed = new(NetworkEventPermission.Everyone);

        public override void OnNetworkSpawn()
        {
            onCodeFailed.Initialize(this);
            onCodeSuccess.Initialize(this);
            onPadPressed.Initialize(this);
            
            foreach (var networkedInteractable in numpadPads)
            {
                networkedInteractable.onInteract.AddListener(() =>
                {
                    HandleKeyPressed(networkedInteractable);
                });
            }
            
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            onCodeFailed?.Dispose();
            onCodeSuccess?.Dispose();
            onPadPressed?.Dispose();
        }

        public void TryKeyCode()
        {
            if (_insertedCode == code)
            {
                onCodeSuccess?.Invoke();
            }
            else
            {
                onCodeFailed?.Invoke();
            }
            _insertedCode = "";
        }
        
        private void HandleKeyPressed(NetworkedInteractable interactable)
        {
            onPadPressed?.Invoke();
            int pressedKey = Array.IndexOf(numpadPads, interactable);
            _insertedCode += pressedKey;

            if (autoSubmit && _insertedCode.Length == code.Length)
            {
                TryKeyCode();
            }
        }
    }
}