using UnityEngine;
using UnityEngine.Events;

namespace Runtime.LevelEvents
{
    /// <summary>
    /// This script is an event driver that invokes an event when a player enters or leaves a trigger volume.
    /// WARNING: While this script supports any kind of collider, it does require at least one collider to be present at any time. Be sure to apply the correct collider shape.
    /// </summary>

    [RequireComponent(typeof(Collider))]
    public class TriggerVolume : MonoBehaviour
    {
        [SerializeField] private bool active = true;
    
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerExit;
        public bool Active
        {
            get => active;
            set
            {
                active = value;
            }
        }

        void OnTriggerEnter(Collider _col)
        {
            if (active)
            {
                onTriggerEnter.Invoke();
            }
        }

        void OnTriggerExit(Collider _col)
        {
            if (active)
            {
                onTriggerExit.Invoke();
            }
        }
    }
}
