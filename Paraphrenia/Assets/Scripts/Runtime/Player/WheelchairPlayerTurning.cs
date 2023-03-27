using UnityEngine;

namespace Runtime.Player
{
    public class WheelchairPlayerTurning : MonoBehaviour
    {
        [SerializeField] private float maxTurn = 20;
        [SerializeField] private float turningSpeed = 2;

        private float _currentRot = 0;
        
        private void Update()
        {
            float horizontalInput = -Input.GetAxis("Horizontal");

            float rot = Mathf.Lerp(_currentRot, horizontalInput * maxTurn, Time.deltaTime * turningSpeed);

            _currentRot = rot;
            transform.rotation = Quaternion.Euler(new Vector3(0,0,_currentRot));
        }
    }
}
