using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MasterLerpComponent : MonoBehaviour
{
    [SerializeField] private EasingDictionary _easingDictionary;
    [SerializeField] private float _lerpDuration = 1f;
    private float _lerpProgress;

    protected void StartLerp()
    {
        _lerpProgress = 0f;
        StartCoroutine(Lerp(Time.deltaTime));
    }

    protected abstract void ApplyLerp(float easeStep);

    private IEnumerator Lerp(float dt)
    {
        while (_lerpProgress < 1)
        {
            _lerpProgress += dt / _lerpDuration;
            float easeStep = _easingDictionary.CalculateEaseStep(_lerpProgress);
            ApplyLerp(easeStep);
            yield return new WaitForSeconds(dt);
        }
    }
}
