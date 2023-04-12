using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Misc
{
    public class Wheelchair : NetworkBehaviour
    {
        [SerializeField, Range(0, 1)] private float lookAtTolerance = .1f;
        [SerializeField] private GameObject playerParent;
        
        private Collider _playerCollider;
        private bool _isGrabbed;

        public UnityEvent onGrab = new();
        public UnityEvent onRelease = new();
        
        /// <summary>
        /// When we enter a trigger we can be pretty sure it's the player, which can then grab us.
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter(Collider other)
        {
            _playerCollider = other;
        }

        /// <summary>
        /// When we exit the trigger we should no longer remember the player, as he shouldn't be able to grab us.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            _playerCollider = null;
        }


        /// <summary>
        /// Handles the grab input and grab's or releases the wheelchair depending on his current state.
        /// you can only grab when you are looking at the wheelchair and are in the trigger collider.
        /// </summary>
        private void Update()
        {
            if (!IsOwner) return;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_isGrabbed)
                {
                    _isGrabbed = false;
                    ReleaseServerRpc();
                    return;
                }
                // if we are not holding it, we cannot release it.
                if (_isGrabbed || playerParent == null) return;
                
                if (Vector3.Dot(transform.forward, playerParent.transform.forward) < 1 - lookAtTolerance) return;

                _isGrabbed = true;
                GrabServerRpc();
            }
        }

        [ServerRpc]
        private void GrabServerRpc()
        {
            transform.parent = playerParent.transform;
            _isGrabbed = true;
            onGrab?.Invoke();
        }

        [ServerRpc]
        private void ReleaseServerRpc()
        {
            transform.parent = null;
            _isGrabbed = false;
            onRelease?.Invoke();
        }
    }
}
