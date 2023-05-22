using System;
using Runtime.Interaction;
using Runtime.Misc;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.Renderers
{
    [RequireComponent(typeof(SequenceInteraction))]
    public class SequenceInteractionGameRenderer : NetworkBehaviour
    {
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material newTargetMaterial;

        private SequenceInteraction _sequenceInteraction;

        private void Awake()
        {
            _sequenceInteraction = GetComponent<SequenceInteraction>();
        }

        public override void OnNetworkSpawn()
        {
            _sequenceInteraction.onNewTargetInteractable.AddListener(RenderNewTarget);
            _sequenceInteraction.onInvalidInteraction.called.AddListener(HandleWrongInteractable);
            
            foreach (var sequenceInteractionInteractable in _sequenceInteraction.Interactables)
            {
                sequenceInteractionInteractable.onInteractorEnter.AddListener(() =>
                    HandleInteractableEnter(sequenceInteractionInteractable));
                sequenceInteractionInteractable.onInteractorExit.AddListener(() =>
                    HandleInteractableExit(sequenceInteractionInteractable));
            }

            base.OnNetworkSpawn();
        }

        private void HandleInteractableEnter(NetworkedInteractable interactable)
        {
            if (PlayerType.Wheelchair.GetNetworkClientID() == NetworkManager.LocalClientId)
            {
                if (_sequenceInteraction.TargetInteractable == interactable)
                {
                    interactable.GetComponent<MeshRenderer>().material = newTargetMaterial;
                }

                return;
            } 
            
            interactable.GetComponent<MeshRenderer>().material = hoverMaterial;
        }

        private void HandleInteractableExit(NetworkedInteractable interactable)
        {
            if (PlayerType.Wheelchair.GetNetworkClientID() == NetworkManager.LocalClientId)
            {
                if (_sequenceInteraction.TargetInteractable == interactable)
                {
                    interactable.GetComponent<MeshRenderer>().material = newTargetMaterial;
                }
                
                return;
            }
            interactable.GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        private void HandleWrongInteractable()
        {
            if (PlayerType.Wheelchair.GetNetworkClientID() != NetworkManager.LocalClientId) return;
            
            foreach (var sequenceInteractionInteractable in _sequenceInteraction.Interactables)
            {
                sequenceInteractionInteractable.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        private void RenderNewTarget(NetworkedInteractable interactable)
        {
            if (PlayerType.Wheelchair.GetNetworkClientID() != NetworkManager.LocalClientId) return;
           
            foreach (var sequenceInteractionInteractable in _sequenceInteraction.Interactables)
            {
                sequenceInteractionInteractable.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
                
            interactable.GetComponent<MeshRenderer>().material = newTargetMaterial;
        }
    }
}