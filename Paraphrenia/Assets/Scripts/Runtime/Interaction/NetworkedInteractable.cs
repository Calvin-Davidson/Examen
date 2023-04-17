using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interaction
{
    /// <summary>
    /// An extension of the interactable class, where rather the firing the events locally we also network the events
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NetworkedInteractable : NetworkBehaviour, IInteractable
    {
        public UnityEvent onInteract;
        public UnityEvent onInteractorEnter;
        public UnityEvent onInteractorExit;

        public bool IsActive { get; set; } = true;

        
        /// <summary>
        /// Called when the interactor can interact with this object
        /// </summary>
        public void InteractorEnter()
        {
            if (!IsActive) return;
            if (IsServer)
            {
                InteractorEnterServerRpc();
            }
            else
            {
                InteractorEnterClientRpc();
            }
        }

        /// <summary>
        /// Called when the interactor can no longer interact with this object.
        /// </summary>
        public void InteractorExit()
        {
            if (!IsActive) return;

            if (IsServer)
            {
                InteractorExitClientRpc();
            }
            else
            {
                InteractorExitServerRpc();
            }
        }

        /// <summary>
        /// Called when the interactor interacts with this object.
        /// </summary>
        public void DoInteract()
        {
            if (!IsActive) return;

            if (IsServer)
            {
                DoInteractClientRpc();
            }
            else
            {
                DoInteractServerRpc();
            }
        }
        
        [ServerRpc]
        private void InteractorEnterServerRpc()
        {
            InteractorEnterClientRpc();
        }
        
        [ClientRpc]
        private void InteractorEnterClientRpc()
        {
            onInteractorEnter?.Invoke();
        }
        
        [ServerRpc]
        private void InteractorExitServerRpc()
        {
            InteractorExitClientRpc();
        }
        
        [ClientRpc]
        private void InteractorExitClientRpc()
        {
            onInteractorExit?.Invoke();
        }
        
        [ServerRpc]
        private void DoInteractServerRpc()
        {
            DoInteractClientRpc();
        }
        
        [ClientRpc]
        private void DoInteractClientRpc()
        {
            onInteract?.Invoke();
        }
    }
}
