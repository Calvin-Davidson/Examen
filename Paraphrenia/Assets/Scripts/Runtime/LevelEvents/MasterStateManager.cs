using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine.Events;

/// <summary>
/// Used to sync the game state and call specific events when the game changes.
/// </summary>

namespace Runtime.LevelEvents
{
    public class MasterStateManager : NetworkBehaviour
    {
        [System.Serializable]
        public class EventContainer
        {
            public int stateIndex;
            public UnityEvent onStateEnter;
            public UnityEvent onStateExit;
        }

        private readonly NetworkVariable<int> _currentState = new();

        public List<EventContainer> container = new();

        // When the network object spawns subscribe to the state value for changes.
        public override void OnNetworkSpawn()
        {
            _currentState.OnValueChanged += HandleStateChange;
        }

        // Unsubscribe from network variable's as this object should no longer exist.
        public override void OnNetworkDespawn()
        {
            _currentState.OnValueChanged -= HandleStateChange;
        }

        // Increases the networked variable of currentState and calls the state enter/exit events for the current and next state.
        public void NextState()
        {
            if (!IsServer) return;
            _currentState.Value += 1;
        }

        // Called when the state changes and calls the respective events.
        // <param name="previous"></param>
        // <param name="newState"></param>
        private void HandleStateChange(int previous, int newState)
        {
            container.FirstOrDefault(eventContainer => eventContainer.stateIndex == previous)?.onStateExit?.Invoke();
            container.FirstOrDefault(eventContainer => eventContainer.stateIndex == newState)?.onStateEnter?.Invoke();
        }
    }
}