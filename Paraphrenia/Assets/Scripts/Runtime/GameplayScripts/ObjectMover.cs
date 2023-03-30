using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Runtime.GameplayScripts
{
    [RequireComponent(typeof(Transform))]
    public class ObjectMover : MasterLerpComponent
    {
        [SerializeField] private bool localSpace = true;
        [SerializeField] private Vector3 targetPosition;

        private Vector3 _defaultPosition;
        private Vector3 _currentPosition;
        private Vector3 _direction;

        private void Awake()
        {
            if (localSpace)
            {
                _defaultPosition = _currentPosition = this.transform.localPosition;
                targetPosition += _defaultPosition;
            }
            else
            {
                _defaultPosition = _currentPosition = this.transform.position;
            }
        }

        public void MoveOn()
        {
            if (localSpace)
            {
                _currentPosition = this.transform.localPosition;
            }
            else
            {
                _currentPosition = this.transform.position;
            }
            _direction = targetPosition - _currentPosition;
            StartLerp();
        }

        public void MoveOff()
        {
            if (localSpace)
            {
                _currentPosition = this.transform.localPosition;
            }
            else
            {
                _currentPosition = this.transform.position;
            }
            _direction = _defaultPosition - _currentPosition;
            StartLerp();
        }

        protected override void ApplyLerp(float easeStep)
        {
            Vector3 result = _direction * easeStep;
            if (localSpace)
            {
                this.transform.localPosition = _currentPosition + result;
            }
            else
            {
                this.transform.position = _currentPosition + result;
            }
        }
    }
}
