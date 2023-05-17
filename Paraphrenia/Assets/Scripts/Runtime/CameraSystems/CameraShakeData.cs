using UnityEngine;

namespace Runtime.CameraSystems
{
    public class CameraShakeData
    {
        private float _positionStrength;
        private float _duration;
        private float _falloffExponent;

        private float _durationLeft;

        public CameraShakeData(float duration, float falloffExponent, float positionStrength)
        {
            _positionStrength = positionStrength;
            _duration = duration;
            _falloffExponent = falloffExponent;
            _durationLeft = _duration;
        }

        public virtual float PositionStrength => _positionStrength * Mathf.Pow((DurationLeft / Duration), FalloffExponent);
        public virtual float Duration => _duration;
        public virtual float DurationLeft => _durationLeft;

        public float FalloffExponent => _falloffExponent;

        public void UpdateShake(float deltaTime)
        {
            _durationLeft -= deltaTime;
        }
    }
}
