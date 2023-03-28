using Runtime.Misc;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Networking
{
    public class NetworkedObjectVisibility : NetworkBehaviour
    {
        [SerializeField] private PlayerType visibleFor;
        
        public UnityEvent onTurnVisible;
        public UnityEvent onTurnInvisible;

        public override void OnNetworkSpawn()
        {
            CheckVisibility();
        }

        [ClientRpc]
        public void SetVisibleForClientRpc(PlayerType playerType)
        {
            visibleFor = playerType;
            CheckVisibility();
        }

        private void CheckVisibility()
        {
            if (visibleFor == PlayerType.Both)
            {
                onTurnVisible?.Invoke();
            }

            if (visibleFor == PlayerType.None)
            {
                onTurnInvisible?.Invoke();
            }
            
            if (NetworkManager.Singleton.LocalClientId == visibleFor.GetNetworkClientID())
            {
                onTurnVisible?.Invoke();
            }
            else
            {
                onTurnInvisible?.Invoke();
            }
        }
    }
}
