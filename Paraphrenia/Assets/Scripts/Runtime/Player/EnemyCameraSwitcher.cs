using System;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.Player
{
    public class EnemyCameraSwitcher : NetworkBehaviour
    {
        [SerializeField] private Camera patientCamera;
        [SerializeField] private Camera enemyCamera;

        [SerializeField] private KeyCode switchKey = KeyCode.Space;
        [SerializeField] private bool shouldHold = false;

        private bool _isPatientView = true;
        
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
            
            switch (shouldHold)
            {
                case true when Input.GetKey(switchKey):
                    SwitchToEnemyCamera();
                    break;
                case true:
                    SwitchToPatientCamera();
                    break;
                default:
                {
                    if (Input.GetKeyDown(switchKey))
                    {
                        SwitchView();
                    }
                    break;
                }
            }
        }

        private void SwitchToPatientCamera()
        {
            patientCamera.enabled = true;
            enemyCamera.enabled = false;
            _isPatientView = true;
        }

        private void SwitchToEnemyCamera()
        {
            patientCamera.enabled = false;
            enemyCamera.enabled = true;
            _isPatientView = false;
        }

        private void SwitchView()
        {
            if (_isPatientView)
            {
                SwitchToEnemyCamera();
            }
            else
            {
                SwitchToPatientCamera();
            }
        }
    }
}
