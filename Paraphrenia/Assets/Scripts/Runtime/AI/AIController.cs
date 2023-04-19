using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable] public enum AIState { Default, Roaming, Chasing, Searching };

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject[] _targets;
    [SerializeField] private float _targetAccuracy = 5;
    [SerializeField] private float _switchTime = 3;

    // Serialized only for debug purposes
    [SerializeField] private int _currentIndex = 0;
    [SerializeField] private AIState _aiState;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _navMeshAgent.destination = _targets[_currentIndex].transform.position;

        if(Vector3.Distance(transform.position,_targets[_currentIndex].transform.position) <= _targetAccuracy)
        {
            StartCoroutine(SelectNewTarget(_currentIndex));
        }
    }

    IEnumerator SelectNewTarget(int oldIndex)
    {
        yield return new WaitForSeconds(_switchTime);
        // Must take into account the chance of selecting the same object over and over, so I itterate 10 attempts to try and select a different target
        int attempts = 0;
        while(_currentIndex==oldIndex && attempts < 10)
        {
            _currentIndex = Random.Range(0, _targets.Length - 1);
            attempts++;
        }
        yield return null;
    }
}
