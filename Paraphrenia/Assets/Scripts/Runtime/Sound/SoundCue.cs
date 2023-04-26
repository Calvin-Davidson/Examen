using UnityEngine;
using System.Linq;

/// <summary>
/// This script is an sound management script. For each sound effect, we use a few variations of sound clips to represent that sound effect.
/// Whenever we want to play that sound effect, we want to randomize which clip is played to increase immersion for the player.
/// This script can however also handle more than just random sound effects. For example, when closing a door, we can choose which sound is played depending on how quickly the door is closed.
/// </summary>

[CreateAssetMenu(fileName = "SC_NewSoundCue", menuName = "Sound Cues/New Sound Cue", order = 1)]
[System.Serializable]
public class SoundCue : ScriptableObject
{
    [System.Serializable]
    public struct SoundCueEntry
    {
        public AudioClip soundClip;
        public int priority;
    }

    [SerializeField] private SoundCueEntry[] soundClips;

    public AudioClip GetRandomClip()
    {
        return soundClips[Random.Range(0, soundClips.Length-1)].soundClip;
    }

    public AudioClip GetLowestPiorityClip()
    {
        return soundClips.OrderBy(i => i.priority).First().soundClip;
    }

    public AudioClip GetHighestPiorityClip()
    {
        return soundClips.OrderBy(i => i.priority).LastOrDefault().soundClip;
    }

    public AudioClip GetNearestPiorityClip(int targetPriority)
    {
        return soundClips.OrderBy(i => IntAbsolute(i.priority - targetPriority)).First().soundClip;
    }

    public AudioClip FloorGetNearestPiorityClip(int targetPriority)
    {
        return soundClips.Where(i => i.priority <= targetPriority).OrderBy(i => IntAbsolute(i.priority - targetPriority)).First().soundClip;
    }

    public AudioClip CeilGetNearestPiorityClip(int targetPriority)
    {
        return soundClips.Where(i => i.priority >= targetPriority).OrderBy(i => i.priority - targetPriority).First().soundClip;
    }

    private int IntAbsolute(int input)
    {
        return input * input / input;
    }
}