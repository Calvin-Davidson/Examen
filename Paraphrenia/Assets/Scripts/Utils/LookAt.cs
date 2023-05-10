using UnityEngine;

namespace Utils
{
    [ExecuteAlways]
    public class LookAt : MonoBehaviour
    {
        [SerializeField] private Transform lookAtTransform;

        [ExecuteAlways]
        private void Update()
        {
            if (lookAtTransform == null) return;
            Debug.Log(1);
            transform.LookAt(lookAtTransform);
        }
    }
}
