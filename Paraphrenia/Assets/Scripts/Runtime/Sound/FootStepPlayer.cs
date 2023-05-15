using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simply plays a sound effect and calls an unity event when the player moves a certain distance.
/// Missing behavior: The ability to swap footstep types depending on walking surface.
/// </summary>

[RequireComponent(typeof(SoundCuePlayer))]
public class FootStepPlayer : MonoBehaviour
{
    [SerializeField] private SurfaceType currentSurfaceType;
    [SerializeField] private SoundCueDictionary soundCueDictionary;
    [SerializeField] private float footStepDistance = 1;

    private SoundCuePlayer _soundCuePlayer;
    private Vector3 _lastPosition;

    public UnityEvent onFootStepPlaced = new();

    void Awake()
    {
        _soundCuePlayer = GetComponent<SoundCuePlayer>();
        _lastPosition = this.transform.position;
    }

    private void Update()
    {
        if ((_lastPosition - this.transform.position).magnitude > footStepDistance)
        {
            _lastPosition = this.transform.position;
            _soundCuePlayer.PlaySound();
            onFootStepPlaced?.Invoke();
        }
    }
}
