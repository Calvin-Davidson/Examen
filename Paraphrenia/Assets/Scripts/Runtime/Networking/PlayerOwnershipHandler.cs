using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking
{
    public class PlayerOwnershipHandler : NetworkBehaviour
    {
        [SerializeField] private ulong playerID;

        public UnityEvent onOwnershipGained = new();
        public UnityEvent onOwnershipLost = new();

        public UnityEvent onSpawnWithoutOwnership = new();
        public UnityEvent onSpawnWithOwnership = new();
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                onSpawnWithoutOwnership?.Invoke();
            }
            else
            {
                onSpawnWithOwnership?.Invoke();
            }
            
            if (!IsServer) return;
            NetworkObject.ChangeOwnership(playerID);
        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
            if (!IsOwner) return;
            onOwnershipGained?.Invoke();
        }

        public override void OnLostOwnership()
        {
            base.OnLostOwnership();
            if (IsOwner) return;
            onOwnershipLost?.Invoke();
        }
    }
}
