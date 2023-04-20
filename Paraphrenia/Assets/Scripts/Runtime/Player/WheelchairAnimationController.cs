using System;
using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(Animator))]
    public class WheelchairAnimationController : MonoBehaviour
    {
        private Transform _cacheTransform;
        private Vector3 _previousPosition;
        private Animator _animator;
        
        private static readonly int Velocity = Animator.StringToHash("Velocity");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _cacheTransform = GetComponent<Transform>();
            _previousPosition = _cacheTransform.position;
        }

        private void FixedUpdate()
        {
            Vector3 currentPos = _cacheTransform.position;
            Vector3 velocity = _previousPosition - currentPos;
            velocity.y = 0;
            
            _animator.SetFloat(Velocity, Vector3.Dot(velocity, _cacheTransform.forward) > 0 ? -velocity.magnitude : velocity.magnitude);
            
            _previousPosition = currentPos;
        }
    }
}