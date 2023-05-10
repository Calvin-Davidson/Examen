using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(SoundCuePlayer))]
public class AIAudioController : MonoBehaviour
{
    private AIController _aiController;
    private SoundCuePlayer _soundCuePlayer;

    [SerializeField] private SoundCue _roamingSoundCue;
    [SerializeField] private SoundCue _chasingSoundCue;
    [SerializeField] private SoundCue _searchingSoundCue;

    private void Awake()
    {
        _aiController = GetComponent<AIController>();
        _soundCuePlayer = GetComponent<SoundCuePlayer>();
        _soundCuePlayer.UpdateSoundCue(_roamingSoundCue);
    }

    private void Update()
    {
        switch (_aiController.aiState)
        {
            case AIState.Chasing:
                _soundCuePlayer.UpdateSoundCue(_chasingSoundCue);
                break;
            case AIState.Searching:
                _soundCuePlayer.UpdateSoundCue(_searchingSoundCue);
                break;
            case AIState.Roaming:
                _soundCuePlayer.UpdateSoundCue(_roamingSoundCue);
                break;
        }
    }
}
