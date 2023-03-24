using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script is a state driver that switches between a On and Off state with random minimum and maximum delays for both states.
/// This function drives a pair of public unity events that other scripts can hook onto, so actions can be performed on state switch.
/// WARNING: It is not recommended to run more than 5 copies of this script at the same time due to performance concerns.
/// </summary>

public class RandomFlicker : MonoBehaviour
{
    public UnityEvent OnFlickerOn;
    public UnityEvent OnFlickerOff;

    [SerializeField] private bool active = true;
    [SerializeField] private float minOffDelay = 0.1f;
    [SerializeField] private float maxOffDelay = 0.6f;
    [SerializeField] private float minOnDelay = 0.02f;
    [SerializeField] private float maxOnDelay = 0.1f;
    public bool Active
    {
        get => active;
        set
        {
            active = value;
            if (value) StartCoroutine(FlickerTimer());
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (active) StartCoroutine(FlickerTimer());
    }
#endif

    private void Start()
    {
        if (active) StartCoroutine(FlickerTimer());
    }
    
    private void OnApplicationQuit()
    {
        active = false;
    }

    private IEnumerator FlickerTimer()
    {
        while (active)
        {
            float delay = Random.Range(minOffDelay, maxOffDelay);
            yield return new WaitForSeconds(delay);

            OnFlickerOn?.Invoke();

            delay = Random.Range(minOnDelay, maxOnDelay);
            yield return new WaitForSeconds(delay);

            OnFlickerOff?.Invoke();
        }
    }
}
