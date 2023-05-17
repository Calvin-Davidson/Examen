using System;
using UnityEngine;

namespace Runtime.CameraSystems
{
    public class CameraShakeImpulse : MonoBehaviour
    {
        [SerializeField] private float positionStrength;
        [SerializeField] private float duration;
        [SerializeField] private float falloffExponent;
        
        public void Pulse()
        {
            CameraShake[] shakeListeners = FindObjectsOfType<CameraShake>();
            foreach (var shakeListener in shakeListeners)
            {
                CameraShakeData data = new CameraShakeData(duration, falloffExponent, positionStrength);
                shakeListener.Shake(data);
            }
        }
    }
}
