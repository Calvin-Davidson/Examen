using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterLerpComponent : MonoBehaviour
{
    [SerializeField] protected bool showDebug = false;
    [SerializeField] private EasingDictionary easingDictionary;
    [SerializeField] private float lerpDuration = 1f;
    private float lerpProgress;

    protected void StartLerp()
    {
        if (showDebug) { Debug.Log("Lerp Started"); }
        lerpProgress = 0f;
        StartCoroutine(Lerp(Time.deltaTime));
    }

    protected virtual void ApplyLerp(float easeStep) { }

    IEnumerator Lerp(float dt)
    {
        while (lerpProgress < 1)
        {
            //if (showDebug) { Debug.Log("Lerp Updating... " + lerpProgress); }
            lerpProgress += dt / lerpDuration;
            float easeStep = easingDictionary.CalculateEaseStep(lerpProgress);
            ApplyLerp(easeStep);
            yield return new WaitForSeconds(dt);
        }
    }
}
