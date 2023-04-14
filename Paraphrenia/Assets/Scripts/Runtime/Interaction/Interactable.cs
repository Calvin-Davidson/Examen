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

        /// <summary>
        /// Called when the interactor can interact with this object
        /// </summary>
        public void InteractorEnter()
        {
            onInteractorEnter?.Invoke();
        }

        /// <summary>
        /// Called when the interactor can no longer interact with this object.
        /// </summary>
        public void InteractorExit()
        {
            onInteractorExit?.Invoke();
        }

        /// <summary>
        /// Called when the interactor interacts with this object.
        /// </summary>
        public void DoInteract()
        {
            onInteract?.Invoke();
        }
    }
}
