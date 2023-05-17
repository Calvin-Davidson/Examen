using UnityEngine;

/// <summary>
/// Simple script that controls object movement based on a lerp conponent.
/// </summary>

namespace Runtime.GameplayScripts
{
    [RequireComponent(typeof(Transform))]
    public class ObjectMover : MasterLerpComponent
    {
        [Tooltip("Double check which target space your target position is in!")]
        [SerializeField] private bool localSpace = true;
        [Tooltip("Double check which target space your target position is in!")]
        [SerializeField] private Vector3 targetPosition;

        private Vector3 _defaultPosition;
        private Vector3 _currentPosition;
        private Vector3 _direction;

        private void Awake()
        {
            if (localSpace) {
                _defaultPosition = _currentPosition = transform.localPosition;
                targetPosition += _defaultPosition; }
            else { _defaultPosition = _currentPosition = transform.position; }
        }

        public void MoveOn()
        {
            if (localSpace) {  _currentPosition = transform.localPosition; }
            else { _currentPosition = transform.position; }
            _direction = targetPosition - _currentPosition;
            StartLerp();
        }

        public void MoveOff()
        {
            if (localSpace) { _currentPosition = transform.localPosition; }
            else { _currentPosition = transform.position; }
            _direction = _defaultPosition - _currentPosition;
            StartLerp();
        }

        protected override void ApplyLerp(float easeStep)
        {
            Vector3 result = _direction * easeStep;
            if (localSpace) { transform.localPosition = _currentPosition + result; }
            else { transform.position = _currentPosition + result; }
        }
    }
}
