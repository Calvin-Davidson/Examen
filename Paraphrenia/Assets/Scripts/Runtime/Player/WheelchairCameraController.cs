using UnityEngine;

namespace Runtime.Player
{
    public class WheelchairCameraController : MonoBehaviour
    {
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;
        [SerializeField] private float lookYLimit = 45.0f;

        [SerializeField] private Camera playerCamera;
        
        void Update()
        {
            float rotationX = -Input.GetAxis("Mouse Y") * lookSpeed;
            float rotationY = Input.GetAxis("Mouse X") * lookSpeed; 

            Vector3 newRotation = playerCamera.transform.localRotation.eulerAngles + new Vector3(rotationX, rotationY, 0);

            if (newRotation.x > lookXLimit && newRotation.x < 360 - lookXLimit)
            {
                float middlePoint = lookXLimit + (360 - lookXLimit) / 2;
                newRotation.x = newRotation.x < middlePoint ? lookXLimit : 360 - lookXLimit;
            }
            
            if (newRotation.y > lookYLimit && newRotation.y < 360 - lookYLimit)
            {
                float middlePoint = lookYLimit + (360 - lookYLimit) / 2;
                newRotation.y = newRotation.y < middlePoint ? lookYLimit : 360 - lookYLimit;
            }
            
            playerCamera.transform.localRotation =
                Quaternion.Euler(newRotation);
        }
    }
}