using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound controller for the enemy AI. Depending on the AI state, play sound effects of a respective sound cue.
/// </summary>

[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(SoundCuePlayer))]
public class AISoundController : MonoBehaviour
{
    [SerializeField] private SoundCue _roamingSoundCue;
    [SerializeField] private SoundCue _chasingSoundCue;
    [SerializeField] private SoundCue _searchingSoundCue;

    private AIController _aiController;
    private SoundCuePlayer _soundCuePlayer;

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
