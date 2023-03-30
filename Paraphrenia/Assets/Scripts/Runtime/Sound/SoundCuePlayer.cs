using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundCuePlayer : MonoBehaviour
{
    public enum CueSelector { Random, LowestPiority, HighestPiority, NearestPiority, FloorNearestPiority, CeilNearestPiority};
    [SerializeField] private SoundCue _soundCue;
    [SerializeField] private CueSelector _cueSelector;
    [SerializeField] private bool _loops;

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
        AudioClip clip = _soundCue.GetRandomClip();

        switch (_cueSelector)
        {
            case CueSelector.LowestPiority:
                {
                    clip = _soundCue.GetLowestPiorityClip();
                    break;
                }
            case CueSelector.HighestPiority:
                {
                    clip = _soundCue.GetHighestPiorityClip();
                    break;
                }
            case CueSelector.NearestPiority:
                {
                    clip = _soundCue.GetNearestPiorityClip(priority);
                    break;
                }
            case CueSelector.FloorNearestPiority:
                {
                    clip = _soundCue.FloorGetNearestPiorityClip(priority);
                    break;
                }
            case CueSelector.CeilNearestPiority:
                {
                    clip = _soundCue.CeilGetNearestPiorityClip(priority);
                    break;
                }
            default: break;
        }

        _audioSource.Play();

        if (!_loops) return;

        StartCoroutine(GetClipEndTime(_audioSource, priority));
    }

    private IEnumerator GetClipEndTime(AudioSource source, int priority = 0)
    {
        var waitForClipRemainingTime = new WaitForSeconds(source.GetClipRemainingTime());
        yield return waitForClipRemainingTime;
        OnClipEnd(priority);
    }

    private void OnClipEnd(int priority = 0)
    {
        PlaySound(priority);
    }
}
