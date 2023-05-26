using Runtime.Networking.NetworkEvent;
using Runtime.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using NetworkEvent = Runtime.Networking.NetworkEvent.NetworkEvent;

namespace Runtime.Misc
{
    public class Wheelchair : NetworkBehaviour
    {
        [SerializeField, Range(0, 1)] private float lookAtTolerance = .1f;
        [SerializeField] private GameObject playerParent;
        
        private Collider _playerCollider;
        private NetworkVariable<bool> _isGrabbed = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private Transform _grabberTransform;
        private Vector3 _previousPosition;
        private Vector3 _previousRotation;

        public NetworkEvent onNetworkedGrab = new(NetworkEventPermission.Owner);
        public NetworkEvent onNetworkedRelease = new(NetworkEventPermission.Owner);

        public NetworkVariable<bool> IsGrabbed => _isGrabbed;

        /// <summary>
        /// When we enter a trigger we can be pretty sure it's the player, which can then grab us.
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            if (other != playerParent.GetComponent<Collider>()) return;
            _playerCollider = other;
        }

        /// <summary>
        /// When we exit the trigger we should no longer remember the player, as he shouldn't be able to grab us.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (other != playerParent.GetComponent<Collider>()) return;
            _playerCollider = null;
        }

        public override void OnNetworkSpawn()
        {
            onNetworkedGrab.Initialize(this);
            onNetworkedRelease.Initialize(this);
            base.OnNetworkSpawn();
        }

        public override void OnDestroy()
        {
            onNetworkedGrab?.Dispose();
            onNetworkedRelease?.Dispose();
        }

        /// <summary>
        /// Handles the grab input and grab's or releases the wheelchair depending on his current state.
        /// you can only grab when you are looking at the wheelchair and are in the trigger collider.
        /// </summary>
        private void Update()
        {
            if (!IsOwner) return;

            if (!Input.GetKeyDown(KeyCode.Space)) return;
            
            if (_isGrabbed.Value)
            {
                DoRelease();
                return;
            }
            // if we are not holding it, we cannot release it.
            if (_isGrabbed.Value || playerParent == null || _playerCollider == null) return;
                
            if (Vector3.Dot(transform.forward, playerParent.transform.forward) < 1 - lookAtTolerance) return;

            DoGrab();
        }

        private void LateUpdate()
        {
            if (_grabberTransform == null) return;
            
            if (_isGrabbed.Value)
            {
                transform.position += _grabberTransform.position - _previousPosition;
                transform.RotateAround(_grabberTransform.position, Vector3.up, (_grabberTransform.eulerAngles - _previousRotation).y);
            }
            
            _previousPosition = _grabberTransform.position;
            _previousRotation = _grabberTransform.eulerAngles;
        }

        private void DoGrab()
        {
            _grabberTransform = _playerCollider.GetComponentInParent<PlayerController>().transform;
            _previousPosition = _grabberTransform.position;
            _previousRotation = _grabberTransform.eulerAngles;
            
            _isGrabbed.Value = true;
            onNetworkedGrab?.Invoke();
        }

        private void DoRelease()
        {
            _grabberTransform = null;
            _isGrabbed.Value = false;
            onNetworkedRelease?.Invoke();
        }
    }
}
