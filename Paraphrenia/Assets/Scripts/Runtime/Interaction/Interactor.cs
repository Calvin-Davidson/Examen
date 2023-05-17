using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interaction
{
    /// <summary>
    /// Base class for the interaction system, handles interactions with the Interactable.cs class.
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Camera interactorCamera;
        [SerializeField] private float interactionDistance = 10;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        private IInteractable _interactable;

        public UnityEvent onInteractableGain;
        public UnityEvent onInteractableLose;
        
        private void Update()
        {
            Ray ray = interactorCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            var didHit = Physics.Raycast(ray, out var hitInfo, interactionDistance);
            var interactable = didHit ? hitInfo.collider.GetComponent<IInteractable>() : null;
            
            if (didHit && interactable is { IsActive: true })
            {
                DoInteractionHit(hitInfo, interactable);
            }
            else
            {
                DoInteractionMiss();
            }

            CheckInteract();
        }

        /// <summary>
        /// Check for user input and if there is a interactable which we want to interact.
        /// </summary>
        private void CheckInteract()
        {
            if (_interactable == null) return;
            if (!Input.GetKeyDown(interactionKey)) return;

            _interactable.DoInteract();
        }

        /// <summary>
        /// Handles the logic for when the interaction ray hit an object.
        /// </summary>
        /// <param name="hitInfo"></param>
        /// <param name="interactable"></param>
        private void DoInteractionHit(RaycastHit hitInfo, IInteractable interactable)
        {
            if (_interactable == interactable) return;
           
            interactable.InteractorEnter();
            _interactable?.InteractorExit();
            _interactable = interactable;
            
            onInteractableGain?.Invoke();
        }

        private void DoInteractionMiss()
        {
            if (_interactable == null) return;

            _interactable.InteractorExit();
            _interactable = null;
            
            onInteractableLose?.Invoke();
        }
    }
}