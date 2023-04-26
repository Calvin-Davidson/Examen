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

        private void CheckVisibility()
        {
            if (visibleFor == PlayerType.Both)
            {
                onTurnVisible?.Invoke();
                return;
            }

            if (visibleFor == PlayerType.None)
            {
                onTurnInvisible?.Invoke();
                return;
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
