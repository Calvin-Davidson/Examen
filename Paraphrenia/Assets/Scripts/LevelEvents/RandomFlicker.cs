using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomFlicker : MonoBehaviour
{
    public UnityEvent OnFlickerOn;
    public UnityEvent OnFlickerOff;
    
    [SerializeField] private float minOffDelay = 0.1f;
    [SerializeField] private float maxOffDelay = 0.6f;
    [SerializeField] private float minOnDelay = 0.02f;
    [SerializeField] private float maxOnDelay = 0.1f;

    private bool active;

    void Start()
    {
        active = true;
        StartCoroutine(FlickerTimer());
    }
    void OnApplicationQuit()
    {
        active = false;
    }

    IEnumerator FlickerTimer()
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
