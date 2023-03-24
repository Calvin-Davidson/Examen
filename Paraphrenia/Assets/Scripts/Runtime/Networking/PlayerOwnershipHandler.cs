using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking
{
    public class PlayerOwnershipHandler : NetworkBehaviour
    {
        [SerializeField] private ulong playerID;

        public UnityEvent onOwnershipGained = new UnityEvent();
        public UnityEvent onOwnershipLost = new UnityEvent();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            NetworkObject.ChangeOwnership(playerID);
        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
            onOwnershipGained?.Invoke();
        }

        public override void OnLostOwnership()
        {
            base.OnLostOwnership();
            onOwnershipLost?.Invoke();
        }
    }
}
