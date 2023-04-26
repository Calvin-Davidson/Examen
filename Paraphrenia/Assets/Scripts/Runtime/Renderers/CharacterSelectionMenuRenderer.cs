using Runtime.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Renderers
{
    public class CharacterSelectionMenuRenderer : NetworkBehaviour
    {
        public UnityEvent onChange = new();
        
        public UnityEvent onBecomeNurse = new();
        public UnityEvent onBecomeWheelchair = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkingController.Instance.IsServerNurse.OnValueChanged += HandleIsServerNurseChange;
            
            HandleIsServerNurseChange(false, NetworkingController.Instance.IsServerNurse.Value);
        }
        
        public void SwitchPlayers()
        {
            if (!IsServer) return;
            NetworkingController.Instance.IsServerNurse.Value = !NetworkingController.Instance.IsServerNurse.Value;
        }

        private void HandleIsServerNurseChange(bool oldValue, bool newValue)
        {
            if (IsServer)
            {
                if (newValue) onBecomeNurse?.Invoke();
                else onBecomeWheelchair?.Invoke();
            }
            else
            {
                if (!newValue) onBecomeNurse?.Invoke();
                else onBecomeWheelchair?.Invoke();
            }
        }
    }
}
