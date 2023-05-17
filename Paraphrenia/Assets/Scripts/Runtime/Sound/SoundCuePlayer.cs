using System.Collections;
using UnityEngine;

/// <summary>
/// Controller script to hook up sound cues to an audio source. This script can decide how the sound files are selected within the cue, as well as looping and looping delay options.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class SoundCuePlayer : MonoBehaviour
{
    public enum CueSelector { Random, LowestPiority, HighestPiority, NearestPiority, FloorNearestPiority, CeilNearestPiority};
    
    [SerializeField] private SoundCue soundCue;
    [Tooltip("What method is used to select sound cue. This only matters if you are feeding a priority value into the sound cue.")]
    [SerializeField] private CueSelector cueSelector;
    [SerializeField] private bool loops;
    [Tooltip("Delay between each loop, in seconds.")]
    [SerializeField] private float loopDelay = 0;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource.playOnAwake)
        {
            PlaySound(0);
        }
    }

    public void PlaySound(int priority = 0)
    {
        AudioClip clip = soundCue.GetRandomClip();

        switch (cueSelector)
        {
            case CueSelector.LowestPiority:
                {
                    clip = soundCue.GetLowestPiorityClip();
                    break;
                }
            case CueSelector.HighestPiority:
                {
                    clip = soundCue.GetHighestPiorityClip();
                    break;
                }
            case CueSelector.NearestPiority:
                {
                    clip = soundCue.GetNearestPiorityClip(priority);
                    break;
                }
            case CueSelector.FloorNearestPiority:
                {
                    clip = soundCue.FloorGetNearestPiorityClip(priority);
                    break;
                }
            case CueSelector.CeilNearestPiority:
                {
                    clip = soundCue.CeilGetNearestPiorityClip(priority);
                    break;
                }
            default: break;
        }
        if(clip != null) _audioSource.clip = clip;
        _audioSource.Play();

        if (!loops) return;

        StartCoroutine(GetClipEndTime(_audioSource, priority));
    }

    public void UpdateSoundCue(SoundCue newSoundCue)
    {
        soundCue = newSoundCue;
    }

    private IEnumerator GetClipEndTime(AudioSource source, int priority = 0)
    {
        var waitForClipRemainingTime = new WaitForSeconds(source.GetClipRemainingTime() + loopDelay);
        yield return waitForClipRemainingTime;
        OnClipEnd(priority);
    }

    private void OnClipEnd(int priority = 0)
    {
        PlaySound(priority);
    }
}
