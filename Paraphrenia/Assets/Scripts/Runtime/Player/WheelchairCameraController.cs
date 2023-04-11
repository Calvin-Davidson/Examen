using System;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.Player
{
    public class WheelchairCameraController : NetworkBehaviour
    {
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;
        [SerializeField] private float lookYLimit = 45.0f;

        [SerializeField] private Camera playerCamera;
        [SerializeField, Tooltip("The object that will change it's parent")] private Transform wheelchairParent;

        private Vector3 _previousForward = Vector3.zero;
        private float _currentYRotation;
        
        private void Awake()
        {
            _previousForward = wheelchairParent.forward;
        }

        void Update()
        {
            if (!IsOwner) return;

            float additionalRotation = Vector3.Angle(_previousForward, wheelchairParent.forward);
            Vector3 cross = Vector3.Cross(_previousForward, wheelchairParent.forward);
            
            if (cross.y < 0) additionalRotation = -additionalRotation;

            _previousForward = wheelchairParent.forward;
            
            float rotationX = -Input.GetAxis("Mouse Y") * lookSpeed;
            float rotationY = Input.GetAxis("Mouse X") * lookSpeed;

            // Limits our y look rotation by keeping an internal value and staying between the set lookYLimit.
            if (_currentYRotation >= lookYLimit && rotationY > 0)
            {
                float overflow = (_currentYRotation + rotationY) % 45f;
                rotationY -= overflow;
            }
            if (_currentYRotation <= -lookYLimit && rotationY < 0)
            {
                float overflow = (_currentYRotation + rotationY) % -45f;
                rotationY -= overflow;
            }
            _currentYRotation += rotationY;
            
            
            Vector3 newRotation = playerCamera.transform.localRotation.eulerAngles + new Vector3(rotationX, rotationY, 0);

            newRotation.y += additionalRotation;

            if (newRotation.x > lookXLimit && newRotation.x < 360 - lookXLimit)
            {
                float middlePoint = lookXLimit + (360 - lookXLimit) / 2;
                newRotation.x = newRotation.x < middlePoint ? lookXLimit : 360 - lookXLimit;
            }
            
            playerCamera.transform.localRotation = Quaternion.Euler(newRotation);
        }
    }
}