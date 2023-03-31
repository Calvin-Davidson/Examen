using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundCuePlayer : MonoBehaviour
{
    public enum CueSelector { Random, LowestPiority, HighestPiority, NearestPiority, FloorNearestPiority, CeilNearestPiority};
    [SerializeField] private SoundCue soundCue;
    [SerializeField] private CueSelector cueSelector;
    [SerializeField] private bool loops;

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
