using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interaction
{
    /// <summary>
    /// The base class for the interaction system, handles interactor enter, exit and interact events.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour, IInteractable
    {
        public UnityEvent onInteract;
        public UnityEvent onInteractorEnter;
        public UnityEvent onInteractorExit;

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Called when the interactor can interact with this object
        /// </summary>
        public void InteractorEnter()
        {
            if (!IsActive) return;
            onInteractorEnter?.Invoke();
        }

        /// <summary>
        /// Called when the interactor can no longer interact with this object.
        /// </summary>
        public void InteractorExit()
        {
            if (!IsActive) return;
            onInteractorExit?.Invoke();
        }

        /// <summary>
        /// Called when the interactor interacts with this object.
        /// </summary>
        public void DoInteract()
        {
            if (!IsActive) return;
            onInteract?.Invoke();
        }
    }
}
