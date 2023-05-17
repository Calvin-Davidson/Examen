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
        [SerializeField] private float maxEnemyCamTime = 10f;
        [SerializeField] private float switchCooldown = 25f;
        
        [SerializeField] private KeyCode switchKey = KeyCode.Space;
        [SerializeField] private bool shouldHold = false;
        [SerializeField] private float switchDelay = .75f;

        private bool _isPatientView = true;
        private bool _shouldBePatientView = true;
        private Coroutine _abilityCrashTimer;
        private Coroutine _switchCoroutine;
        private float _timeSinceLastSwitch = -float.PositiveInfinity;
        
        public UnityEvent onSwitchStart = new();
        public UnityEvent onSwitchComplete = new();
        public UnityEvent onSwitchCancelled = new();
        public UnityEvent onSwitchFailed = new();
        public UnityEvent<float> onSwitchProgress = new();
        public UnityEvent onEnemyEyeCrash;

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
                if (_abilityCrashTimer != null) StopCoroutine(_abilityCrashTimer);
                patientCamera.enabled = true;
                _isPatientView = true;
                enemyCamera.enabled = false;
            }
            else
            {
                patientCamera.enabled = false;
                _isPatientView = false;
                enemyCamera.enabled = true;
                if (_abilityCrashTimer != null) StopCoroutine(_abilityCrashTimer);
                _abilityCrashTimer = StartCoroutine(EnemyEyeCrashTimer());
            }
            
            onSwitchComplete?.Invoke();
        }

        private IEnumerator EnemyEyeCrashTimer()
        {
            yield return new WaitForSeconds(maxEnemyCamTime);
            if (_shouldBePatientView) yield break;
            StopCoroutine(_switchCoroutine);
            SwitchView();
            onEnemyEyeCrash?.Invoke();
        }
        
        private void SwitchView()
        {
            
            if (_timeSinceLastSwitch + switchCooldown > Time.time && _isPatientView)
            {
                onSwitchFailed?.Invoke();
                return;
            }
            
            _shouldBePatientView = !_shouldBePatientView;
            
            if (ShouldCancelSwitch())
            {
                onSwitchCancelled?.Invoke();
                CancelSwitch();
                return;
            }

            _timeSinceLastSwitch = Time.time;
            onSwitchStart?.Invoke();
            _switchCoroutine = StartCoroutine(SwitchWithDelay());
        }

        private bool ShouldCancelSwitch()
        {
            return _isPatientView && _shouldBePatientView;
        }

        private void CancelSwitch()
        {
            StopCoroutine(_switchCoroutine);
        }
    }
}
