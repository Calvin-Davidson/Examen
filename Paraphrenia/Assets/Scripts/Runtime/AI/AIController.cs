using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script is a basic state-based controller for an AI actor.
/// This AI is able to patrol passively within a scene, while using a field of view script to detect chase targets.
/// Once a target is found, the AI will chase it in attempts to catch it.
/// If line of sight is lost to a target, the AI will navigate to the last known location of the target, and then circle in place for a small duration.
/// If the AI find the target again, or another target, during the search, it will go back to chasing. Otherwise, it will return to patrol behavior.
/// </summary>

[System.Serializable] public enum AIState { Default, Roaming, Chasing, Searching };

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FieldOfView))]
public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject[] _targets;
    [SerializeField] private float _targetAccuracy = 5;
    [SerializeField] private float _switchTime = 3;
    [SerializeField] private float _searchTime = 10;
    [SerializeField] private float _aggroTime = 2;
    [SerializeField] private float _aggroDecayRate = 0.25f;

    // Serialized only for debug purposes
    [SerializeField] private int _currentIndex = 0;
    [SerializeField] private AIState _aiState = AIState.Roaming;
    [SerializeField] private float _timeSinceLastChase;
    [SerializeField] private float _accumulatedAggro;

    private NavMeshAgent _navMeshAgent;
    private FieldOfView _fieldOfView;
    private Vector3 _lastKnownTargetPosition;
    private bool _invertSearchPattern;
    
    

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
        _aiState = AIState.Roaming;
    }

    private void Update()
    {
        // Update AI State
        if(_fieldOfView.visibleTargets.Count > 0)
        {
            if(_accumulatedAggro < _aggroTime)
            {
                _accumulatedAggro += Time.deltaTime;
                if (_accumulatedAggro > _aggroTime) _accumulatedAggro = _aggroTime;
                Debug.Log("Accumulating aggro... " + _accumulatedAggro);
            }

            if(_accumulatedAggro >= _aggroTime)
            {
                _aiState = AIState.Chasing;
                _timeSinceLastChase = 0;
            }
        }
        else if(_aiState == AIState.Chasing)
        {
            _aiState = AIState.Searching;
            _invertSearchPattern = RandomBool();
        }
        else if(_aiState == AIState.Searching && _timeSinceLastChase >= _searchTime)
        {
            _aiState = AIState.Roaming;
        }
        else if (_aiState == AIState.Roaming)
        {
            _accumulatedAggro -= Time.deltaTime * _aggroDecayRate;
            if (_accumulatedAggro < 0) _accumulatedAggro = 0;
        }

        switch (_aiState)
        {
            case AIState.Chasing:
            {
                OnChase();
                break;
            }
            case AIState.Searching:
            {
                OnSearch();
                break;
            }
            case AIState.Roaming:
            {
                OnRoam();
                break;
            }
            default:
            {
                break;
            }
        }
    }
    
    // Update functions per state
    private void OnChase()
    {
        // Update last known target position
        float distance = _fieldOfView.viewRadius + 1;
        foreach (Transform _transform in _fieldOfView.visibleTargets)
        {
            // Due to Field Of View not being updated every frame for performance reasons, we check whether the target has moved outside of line of sight
            Vector3 directionToTarget = (_transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) > _fieldOfView.viewAngle / 2)
            {
                return; // Target is outside of view angle
            }
            float distanceToTarget = Vector3.Distance(transform.position, _transform.position);
            if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _fieldOfView.obstacleMask))
            {
                return; // Target is hidden by obstacle
            }

            // Only chase the closest possible target
            if (Vector3.Distance(transform.position, _transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, _transform.position);
                _lastKnownTargetPosition = _transform.position;
            }
        }
        _navMeshAgent.destination = _lastKnownTargetPosition;

        if (Vector3.Distance(transform.position, _lastKnownTargetPosition) <= _targetAccuracy)
        {
            Debug.Log("Caught a player!");
        }
    }

    private void OnSearch()
    {
        _navMeshAgent.destination = _lastKnownTargetPosition + RotationalVector(_timeSinceLastChase, _invertSearchPattern, 0.5f, 2f);
        _timeSinceLastChase += Time.deltaTime;
    }

    private void OnRoam()
    {
        _navMeshAgent.destination = _targets[_currentIndex].transform.position;

        if (Vector3.Distance(transform.position, _targets[_currentIndex].transform.position) <= _targetAccuracy)
        {
            StartCoroutine(SelectNewTarget(_currentIndex));
        }
    }

    // Simple function that creates a vector that "rotates" around another vector based on time.
    private Vector3 RotationalVector(float time, bool invert = false, float rotationRadius = 1, float rotationRate = 2)
    {
        Vector3 rotationalVector = new Vector3(Mathf.Sin(time * rotationRate), 0, Mathf.Cos(time * rotationRate));
        
        if (invert)
        {
            rotationalVector = new Vector3(-Mathf.Sin(time * rotationRate), 0, Mathf.Cos(time * rotationRate));
        }

        return rotationalVector * rotationRadius;
    }

    private bool RandomBool()
    {
        return Random.value >= 0.5;
    }

    private IEnumerator SelectNewTarget(int oldIndex)
    {
        yield return new WaitForSeconds(_switchTime);

        // Must take into account the chance of selecting the same object over and over, so I itterate 10 attempts to try and select a different target
        int attempts = 0;
        while(_currentIndex==oldIndex && attempts < 10)
        {
            _currentIndex = Random.Range(0, _targets.Length - 1);
            attempts++;
        }
    }
}
