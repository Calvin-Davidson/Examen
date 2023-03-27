using UnityEngine;

namespace Runtime.GameplayScripts
{
    /// <summary>
    /// This script is a basic controller script that can drive light intensity through an On and Off state.
    /// There is support for a color override, though the light's default configured color can also be used.
    /// This script is meant to be hooked up to the RandomFlicker.cs manager script.
    /// WARNING: When setting intensity through code, you MUST configure the light with the intensity unit "Nits".
    /// </summary>

    [RequireComponent(typeof(Light))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private bool overrideColor = false;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float offIntensity = 5000; 
        [SerializeField] private float onIntensity = 9000;

        private Light light;

        private void Awake()
        {
            light = GetComponent<Light>();
            if (overrideColor)
            {
                light.color = color;
            }
        }

        public void TurnLightOn()
        {
            if(light != null) light.intensity = onIntensity;
        }

        public void TurnLightOff()
        {
            if (light != null) light.intensity = offIntensity;
        }
    }
}
