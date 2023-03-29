using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovementComponent : MasterVectorLerpComponent
{
    public LerpSettings lerpSettings;

    protected override float CalculateEaseStep(float currentPercent)
    {
        return lerpSettings.easingDictionary.CalculateEaseStep(currentPercent, lerpSettings.lerpType);
    }
    protected override void ApplyLerp(Vector3 result)
    {
        this.transform.position = _startVector + result;
    }
}
