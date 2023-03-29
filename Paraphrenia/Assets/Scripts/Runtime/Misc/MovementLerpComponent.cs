using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLerpComponent : VectorLerpComponent
{
    protected override void ApplyLerp(Vector3 result)
    {
        this.transform.position = _startVector + result;
    }
}
