using UnityEngine;

namespace Runtime.Player
{
    public class WheelchairPlayerTurning : MonoBehaviour
    {
        [SerializeField] private float maxTurnZ = 20;
        [SerializeField] private float maxTurnX = 20;
        [SerializeField] private float turningSpeed = 2;

        private float _currentRotZ = 0;
        private float _currentRotX = 0;
        
        private void Update()
        {
            float horizontalInput = -Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            
            _currentRotZ = Mathf.Lerp(_currentRotZ, horizontalInput * maxTurnZ, Time.deltaTime * turningSpeed);
            _currentRotX = Mathf.Lerp(_currentRotX, verticalInput * maxTurnX, Time.deltaTime * turningSpeed);
            
            transform.rotation = Quaternion.Euler(new Vector3(_currentRotX,0,_currentRotZ));
        }
    }
}
