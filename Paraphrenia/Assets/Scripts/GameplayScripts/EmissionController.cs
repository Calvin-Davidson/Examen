using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Renderer renderer;

    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float offIntensity = 4;
    [SerializeField] private float onIntensity = 15;
    

    void Start()
    {
        if (!overrideColor)
        {
            float intensity = renderer.material.GetFloat("_EmissiveIntensity");
            color = renderer.material.GetColor("_EmissiveColor");
            color /= intensity;
        }
    }

    public void TurnLightOn()
    {
        renderer.material.SetColor("_EmissiveColor", color * Mathf.Pow(2,onIntensity));
    }

    public void TurnLightOff()
    {
        renderer.material.SetColor("_EmissiveColor", color * Mathf.Pow(2, offIntensity));
    }
}
