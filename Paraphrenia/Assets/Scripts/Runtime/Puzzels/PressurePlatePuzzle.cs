using System.Collections.Generic;
using Runtime.LevelEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Puzzels
{
    public class PressurePlatePuzzle : Puzzle
    {
        [SerializeField] private List<NetworkedTriggerVolume> pressurePlates = new();
        
        private bool _isActive = false;
        private int _currentTarget = 0;

        public UnityEvent onValidEnter;
        public UnityEvent onInvalidEnter;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                // we are inactive and switch to being active. Makes sure that when we are already active is does not break.
                if (!_isActive && value)
                {
                    _currentTarget = 0;
                }
                _isActive = value;
            }
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            foreach (var networkedTriggerVolume in pressurePlates)
            {
                networkedTriggerVolume.onTriggerEnter.AddListener(() => OnPressurePlateEnter(networkedTriggerVolume));
            }
        }

        private void OnPressurePlateEnter(NetworkedTriggerVolume volume)
        {
            if (!IsActive) return;
            if (volume == pressurePlates[_currentTarget])
            {
                _currentTarget += 1;
                HandleValidPressurePlate();
                CheckCompletion();
            }
            else
            {
                HandleInvalidPressurePlate();
            }
        }

        private void HandleInvalidPressurePlate()
        {
            onInvalidEnter?.Invoke();
        }

        private void HandleValidPressurePlate()
        {
            onValidEnter?.Invoke();            
        }

        private void CheckCompletion()
        {
            if (!IsServer) return;
            if (_currentTarget == pressurePlates.Count) DoCompleteClientRpc();
        }
    }
}
