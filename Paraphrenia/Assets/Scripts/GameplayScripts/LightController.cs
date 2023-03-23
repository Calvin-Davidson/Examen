using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light light;

    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float offIntensity = 5000;
    [SerializeField] private float onIntensity = 9000;
    

    void Start()
    {
        if (overrideColor)
        {
            light.color = color;
        }
    }

    public void TurnLightOn()
    {
        light.intensity = onIntensity;
    }

    public void TurnLightOff()
    {
        light.intensity = offIntensity;
    }
}
