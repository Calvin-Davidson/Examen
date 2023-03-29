using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LerpSettings
{
    [HideInInspector]
    public EasingDictionary easingDictionary = new EasingDictionary();
    public EasingDictionary.LerpType lerpType;
}
