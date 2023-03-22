using System;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Misc
{
    public class Wheelchair : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float lookAtTolerance = .1f;
        
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_isGrabbed)
                {
                    transform.parent = null;
                    _isGrabbed = false;
                    onRelease?.Invoke();
                }
                // if we are not holding it, we cannot release it.
                if (_isGrabbed || _playerCollider == null) return;
                
                if (Vector3.Dot(transform.forward, _playerCollider.transform.forward) < 1 - lookAtTolerance) return;
                    
                transform.parent = _playerCollider.gameObject.transform;
                _isGrabbed = true;
                onGrab?.Invoke();
            }
        }
    }
}
