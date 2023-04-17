using Runtime.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Puzzels
{
    public class ElectricRoomPuzzle : Puzzle
    {
        [SerializeField, Space] private NetworkedInteractable breakerInteractable;
        [SerializeField] private NetworkedInteractable[] interactableCables;

        private int _repairedCables = 0;
        private bool _isValid = false;
        
        public UnityEvent onBreakerFailed;
        public UnityEvent onBreakerSuccess;
        public UnityEvent onBecameValid;
        public UnityEvent onBecameInvalid;

        private bool IsValid
        {
            get => _isValid;
            set
            {
                if (value) onBecameValid?.Invoke();
                if (!value) onBecameInvalid?.Invoke();
                _isValid = value;
            }
        }

        public override void OnNetworkSpawn()
        {
            breakerInteractable.onInteract.AddListener(HandleBreakerInteract);
            foreach (var networkedInteractable in interactableCables)
            {
                networkedInteractable.onInteract.AddListener(HandleCableInteract);
            }
            base.OnNetworkSpawn();
        }
        

        private void HandleBreakerInteract()
        {
            if (IsValid)
            {
                onBreakerSuccess?.Invoke();
            }
            else
            {
                onBreakerFailed?.Invoke();
            }
        }

        private void HandleCableInteract()
        {
            _repairedCables += 1;
            if (_repairedCables == interactableCables.Length - 1) IsValid = true;
        }
    }
}
