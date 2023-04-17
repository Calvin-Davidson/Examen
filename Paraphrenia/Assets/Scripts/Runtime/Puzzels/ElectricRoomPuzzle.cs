using Runtime.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Puzzels
{
    public class ElectricRoomPuzzle : Puzzle
    {
        [SerializeField] private NetworkedInteractable breakerInteractable;

        public UnityEvent onBreakerFailed;
        public UnityEvent onBreakerSuccess;
        
        public bool IsValid { get; set; } = false;

        public override void OnNetworkSpawn()
        {
            breakerInteractable.onInteract.AddListener(OnBreakerInteract);
            base.OnNetworkSpawn();
        }
        

        private void OnBreakerInteract()
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
    }
}
