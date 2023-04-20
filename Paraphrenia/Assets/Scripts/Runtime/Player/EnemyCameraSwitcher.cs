using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Player
{
    public class EnemyCameraSwitcher : NetworkBehaviour
    {
        [SerializeField] private Camera patientCamera;
        [SerializeField] private Camera enemyCamera;

        [SerializeField] private KeyCode switchKey = KeyCode.Space;
        [SerializeField] private bool shouldHold = false;
        [SerializeField] private float switchDelay = .75f;

        public UnityEvent onSwitchStart = new();
        public UnityEvent onSwitchComplete = new();
        public UnityEvent onSwitchCancelled = new();
        public UnityEvent<float> onSwitchProgress = new();


        private bool _isPatientView = true;
        private bool _shouldBePatientView = true;

        private void Awake()
        {
            if (patientCamera == null)
            {
                Debug.LogError("Cannot switch camera's with the patientCamera is not defined");
                enabled = false;
            }
            if (enemyCamera == null)
            {
                Debug.LogError("Cannot switch camera's with the enemyCamera is not defined");
                enabled = false;
            }
        }


        private void Update()
        {
            if (!IsOwner) return;

            if (shouldHold)
            {
                if (Input.GetKey(switchKey) && _shouldBePatientView)
                {
                    SwitchView();
                }
                if (!Input.GetKey(switchKey) && !_shouldBePatientView)
                {
                    SwitchView();
                }
            }
            
            if (!shouldHold && Input.GetKeyDown(KeyCode.Space))
            {
                SwitchView();
            }
        }

        private IEnumerator SwitchWithDelay()
        {
            float progress = 0;

            while (progress < switchDelay)
            {
                progress += Time.deltaTime;
                onSwitchProgress.Invoke(progress / switchDelay);
                yield return null;
            }
            
            if (!_isPatientView && _shouldBePatientView)
            {
                patientCamera.enabled = true;
                _isPatientView = true;
                enemyCamera.enabled = false;
            }
            else
            {
                patientCamera.enabled = false;
                _isPatientView = false;
                enemyCamera.enabled = true;
            }
            
            onSwitchComplete?.Invoke();
        }
        
        private void SwitchView()
        {
            _shouldBePatientView = !_shouldBePatientView;
            
            if (ShouldCancelSwitch())
            {
                onSwitchCancelled?.Invoke();
                CancelSwitch();
                return;
            }
         
            onSwitchStart?.Invoke();
            StartCoroutine(SwitchWithDelay());
        }

        private bool ShouldCancelSwitch()
        {
            return _isPatientView && _shouldBePatientView;
        }

        private void CancelSwitch()
        {
            StopAllCoroutines();
        }
    }
}
