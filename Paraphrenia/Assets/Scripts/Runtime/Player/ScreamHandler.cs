using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class ScreamHandler : MonoBehaviour
    {
        [SerializeField] private KeyCode screamKey = KeyCode.Space;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(screamKey))
            {
                _audioSource.Play();
            }
    
            if (Input.GetKeyUp(screamKey))
            {
                _audioSource.Pause();
            }
        }
    }
}
