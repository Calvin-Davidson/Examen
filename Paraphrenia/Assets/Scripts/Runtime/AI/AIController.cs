using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// This script is a basic state-based controller for an AI actor.
/// This AI is able to patrol passively within a scene, while using a field of view script to detect chase targets.
/// Once a target is found, the AI will chase it in attempts to catch it.
/// If line of sight is lost to a target, the AI will navigate to the last known location of the target, and then circle in place for a small duration.
/// If the AI find the target again, or another target, during the search, it will go back to chasing. Otherwise, it will return to patrol behavior.
/// </summary>

[System.Serializable] public enum AIState { Default, Roaming, Chasing, Searching, ForcedHunt };

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FieldOfView))]
public class AIController : MonoBehaviour
{
    public AIState aiState = AIState.Roaming;

    [SerializeField] private GameObject[] targets;
    [Tooltip("How close the AI will try to get to a roam target, in meters.")]
    [SerializeField] private float targetAccuracy = 5;
    [Tooltip("How long the AI will stay at a roam target, before selecting a new roam target, in seconds.")]
    [SerializeField] private float switchTime = 3;
    [Tooltip("How long the AI will stay in search state, in seconds.")]
    [SerializeField] private float searchTime = 10;
    [Tooltip("How long the AI needs to see a target to switch to chase state, in seconds.")]
    [SerializeField] private float aggroTime = 1;
    [Tooltip("Multiplier to the decay rate of accumulated aggro.")]
    [SerializeField] private float aggroDecayRate = 1;
    [Tooltip("The radius of the circle the AI will walk during search.")]
    [SerializeField] private float searchRadius = 1;
    [Tooltip("The rate at which the AI will follow the circle when searching.")]
    [SerializeField] private float searchRotationRate = 1;

    private bool _invertSearchPattern;
    private bool _didCatchPlayer = false;
    private int _currentIndex = 1;
    private float _timeSinceLastChase;
    private float _accumulatedAggro;

    private NavMeshAgent _navMeshAgent;
    private FieldOfView _fieldOfView;
    private Vector3 _lastKnownTargetPosition;
    
    public UnityEvent onCaughtPlayer = new();

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
    }

    private void Update()
    {
        // Update AI State
        if(_fieldOfView.visibleTargets.Count > 0)
        {
            if(_accumulatedAggro < aggroTime)
            {
                _accumulatedAggro += Time.deltaTime;
                if (_accumulatedAggro > aggroTime) _accumulatedAggro = aggroTime;
            }

            if(_accumulatedAggro >= aggroTime)
            {
                aiState = AIState.Chasing;
                _timeSinceLastChase = 0;
            }
        }
        else if(aiState == AIState.Chasing)
        {
            aiState = AIState.Searching;
            _invertSearchPattern = RandomBool();
        }
        else if(aiState == AIState.Searching && _timeSinceLastChase >= searchTime)
        {
            aiState = AIState.Roaming;
        }
        else if (aiState == AIState.Roaming)
        {
            _accumulatedAggro -= Time.deltaTime * aggroDecayRate;
            if (_accumulatedAggro < 0) _accumulatedAggro = 0;
        }

        switch (aiState)
        {
            case AIState.Chasing:
                OnChase();
                break;
            case AIState.Searching:
                OnSearch();
                break;
            case AIState.Roaming:
                OnRoam();
                break;
            case AIState.ForcedHunt:
                OnForcedHunt();
                break;
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

        _navMeshAgent.SetDestination(_lastKnownTargetPosition);
        
        if (Vector3.Distance(transform.position, _lastKnownTargetPosition) <= targetAccuracy && !_didCatchPlayer)
        {
            _didCatchPlayer = true;
            onCaughtPlayer?.Invoke();
            Debug.Log("[AIController] Caught a player!");
        }
    }

    private void OnSearch()
    {
        _navMeshAgent.SetDestination(_lastKnownTargetPosition + RotationalVector(_timeSinceLastChase, _invertSearchPattern, searchRadius, searchRotationRate));
        _timeSinceLastChase += Time.deltaTime;
    }

    private void OnRoam()
    {
        _navMeshAgent.SetDestination(targets[_currentIndex].transform.position);

        if (Vector3.Distance(transform.position, targets[_currentIndex].transform.position) <= targetAccuracy)
        {
            StartCoroutine(SelectNewTarget(_currentIndex));
        }
    }

    private void OnForcedHunt()
    {
        _navMeshAgent.SetDestination(_lastKnownTargetPosition);

        if (Vector3.Distance(transform.position, _lastKnownTargetPosition) <= targetAccuracy)
        {
            _timeSinceLastChase = 0;
            aiState = AIState.Searching;
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

    public void ForceNewTarget(Transform transform)
    {
        _lastKnownTargetPosition = transform.position;
        aiState = AIState.ForcedHunt;
    }

    private IEnumerator SelectNewTarget(int oldIndex)
    {
        yield return new WaitForSeconds(switchTime);

        // Must take into account the chance of selecting the same object over and over, so I itterate 10 attempts to try and select a different target
        int attempts = 0;
        while(_currentIndex==oldIndex && attempts < 10)
        {
            _currentIndex = Random.Range(0, targets.Length - 1);
            attempts++;
        }
    }
}
