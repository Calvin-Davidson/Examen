using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script is an event driver that invokes an event when a player enters or leaves a trigger volume.
/// WARNING: While this script supports any kind of collider, it does require at least one collider to be present at any time. Be sure to apply the correct collider shape.
/// </summary>

[RequireComponent(typeof(Collider))]
public class TriggerVolume : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    [SerializeField] private bool active = true;

    private Collider collider;

    public bool Active
    {
        get => active;
        set
        {
            active = value;
        }
    }

    void Awake()
    {
        collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (active)
        {
            onTriggerEnter.Invoke();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (active)
        {
            onTriggerExit.Invoke();
        }
    }
}
