using Runtime.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Puzzels
{
    public class ElectricRoomPuzzle : Puzzle
    {
        [SerializeField, Space] private NetworkedInteractable breakerInteractable;

        [SerializeField, Tooltip("Should be filled in the order in which the should be fixed")]
        private NetworkedInteractable[] interactableCables;

        private int _repairedCables = 0;
        private bool _isValid = false;

        public UnityEvent onBreakerFailed;
        public UnityEvent onBreakerSuccess;
        public UnityEvent onBecameValid;
        public UnityEvent onBecameInvalid;
        public UnityEvent onInvalidCableRepaired;

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
                networkedInteractable.onInteract.AddListener(() =>
                {
                    HandleCableInteract(networkedInteractable);
                });
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
                DoFail();
                onBreakerFailed?.Invoke();
            }
        }

        private void HandleCableInteract(NetworkedInteractable interactable)
        {
            if (interactableCables[_repairedCables] != interactable)
            {
                DoFail();
                onInvalidCableRepaired?.Invoke();
                return;
            }

            interactable.IsActive = false;
            _repairedCables += 1;
            if (_repairedCables == interactableCables.Length) IsValid = true;
        }

        /// <summary>
        /// Can be invoked when part of the puzzle fails will reset the interactable cables and set repaired cables to 0
        /// </summary>
        private void DoFail()
        {
            _repairedCables = 0;
            foreach (var networkedInteractable in interactableCables)
            {
                networkedInteractable.IsActive = true;
            }
        }
    }
}