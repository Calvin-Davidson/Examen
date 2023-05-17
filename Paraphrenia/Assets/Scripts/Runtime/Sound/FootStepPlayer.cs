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
    [Tooltip("Sound Cues sorted by surface types.")]
    [SerializeField] private SoundCueDictionary soundCueDictionary;
    [Tooltip("The distance the objcet needs to move to trigger a sound effect, in meters.")]
    [SerializeField] private float footStepDistance = 1;

    private Vector3 _lastPosition;
    private SoundCuePlayer _soundCuePlayer;

    public UnityEvent onFootStepPlaced = new();

    void Awake()
    {
        _soundCuePlayer = GetComponent<SoundCuePlayer>();
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if ((_lastPosition - transform.position).magnitude > footStepDistance)
        {
            _lastPosition = transform.position;
            _soundCuePlayer.PlaySound();
            onFootStepPlaced?.Invoke();
        }
    }
}
