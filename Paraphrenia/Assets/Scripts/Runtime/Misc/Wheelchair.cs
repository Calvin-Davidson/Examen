using System;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Misc
{
    public class Wheelchair : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float lookAtTolerance = .1f;
        
        private Collider _playerCollider;
        private bool _isGrabbed = false;

        public UnityEvent onGrab = new();
        public UnityEvent onRelease = new();
        
        public void OnTriggerEnter(Collider other)
        {
            _playerCollider = other;
        }

        private void OnTriggerExit(Collider other)
        {
            _playerCollider = null;
        }


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
