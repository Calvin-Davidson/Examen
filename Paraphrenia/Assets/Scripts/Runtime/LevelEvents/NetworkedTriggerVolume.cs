using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.LevelEvents
{
    /// <summary>
    /// This is the networked version of the TriggerVolume, it works the same except that only the server can handle the respective events.
    /// WARNING: While this script supports any kind of collider, it does require at least one collider to be present at any time. Be sure to apply the correct collider shape.
    /// </summary>

    [RequireComponent(typeof(Collider))]
    public class NetworkedTriggerVolume : NetworkBehaviour
    {
        [SerializeField] private bool active = true;
    
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerExit;

        public UnityEvent onLastTriggerExit;
        public UnityEvent onFirstTriggerEnter;

        private int _activeTriggers = 0;
        

        public bool Active
        {
            get => active;
            set => active = value;
        }
        
        void OnTriggerEnter(Collider _)
        {
            if (_activeTriggers == 0) HandleFirstTriggerEnterClientRpc();
            _activeTriggers += 1;
            if (active && IsServer)
            {
                HandleTriggerEnterClientRpc();
            }
        }

        void OnTriggerExit(Collider _)
        {
            if (_activeTriggers == 1) HandleLastTriggerExitClientRpc();
            _activeTriggers -= 1;
            if (active && IsServer)
            {
                HandleTriggerExitClientRpc();
            }
        }

        [ClientRpc]
        private void HandleTriggerExitClientRpc()
        {
            onTriggerExit.Invoke();
        }

        [ClientRpc]
        private void HandleTriggerEnterClientRpc()
        {
            onTriggerEnter.Invoke();
        }

        [ClientRpc]
        private void HandleFirstTriggerEnterClientRpc()
        {
            onFirstTriggerEnter?.Invoke();
        }

        [ClientRpc]
        private void HandleLastTriggerExitClientRpc()
        {
            onLastTriggerExit?.Invoke();
        }
    }
}