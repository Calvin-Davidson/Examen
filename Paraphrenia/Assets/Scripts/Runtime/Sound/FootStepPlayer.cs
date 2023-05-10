using UnityEngine;
using UnityEngine.Events;

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
