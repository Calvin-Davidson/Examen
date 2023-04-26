using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
