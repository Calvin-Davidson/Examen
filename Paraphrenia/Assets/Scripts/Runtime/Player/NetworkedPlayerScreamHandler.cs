using Unity.Netcode;
using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class NetworkedPlayerScreamHandler : NetworkBehaviour
    {
        [SerializeField] private KeyCode screamKey = KeyCode.Space;

        private AudioSource _audioSource;
        private readonly NetworkVariable<bool> _isScreaming = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnNetworkSpawn()
        {
            _isScreaming.OnValueChanged += HandleIsScreamingChange;
        }

        private void Update()
        {
            if (!NetworkObject.IsOwner)
            {
                return;
            }
           
            if (Input.GetKeyDown(screamKey))
            {
                _isScreaming.Value = true;
            }
    
            if (Input.GetKeyUp(screamKey))
            {
                _isScreaming.Value = false;
            }
        }

        private void HandleIsScreamingChange(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                _audioSource.Play();
            }
    
            if (oldValue)
            {
                _audioSource.Pause();
            }
        }
    }
}
