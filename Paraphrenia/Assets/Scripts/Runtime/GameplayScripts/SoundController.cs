using UnityEngine;

/// <summary>
/// Simple controller to control the volume of an audio source in a turn on and turn off fashion.
/// The reason we use volume instead of enabling and disabling the object is to not pause the audio clip itself, which would have caused a variety of issues within the project.
/// </summary>

namespace Runtime.GameplayScripts
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        private AudioSource _audioSource;
        private float _defaultVolume = 0.5f;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _defaultVolume = _audioSource.volume;
        }

        public void SoundOn()
        {
            _audioSource.volume = _defaultVolume;
        }

        public void SoundOff()
        {
            _audioSource.volume = 0.001f;
        }
    }
}
