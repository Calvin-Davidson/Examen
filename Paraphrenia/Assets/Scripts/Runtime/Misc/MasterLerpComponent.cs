using System.Collections;
using UnityEngine;

public abstract class MasterLerpComponent : MonoBehaviour
{
    [SerializeField] private EasingDictionary easingDictionary;
    [SerializeField] private float lerpDuration = 1f;

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
            _lerpProgress += dt / lerpDuration;
            float easeStep = easingDictionary.CalculateEaseStep(_lerpProgress);
            ApplyLerp(easeStep);
            yield return new WaitForSeconds(dt);
        }
    }
}
