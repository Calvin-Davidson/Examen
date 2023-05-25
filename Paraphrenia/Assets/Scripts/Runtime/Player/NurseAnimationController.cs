using Runtime.Misc;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.Player
{
    public class NurseAnimationController : NetworkBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody characterRigidbody;
        [SerializeField] private Wheelchair wheelchair;
        

        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");
        private const float MinWalkMagnitude = 0.1f;

        private NetworkVariable<bool> _isWalking = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void Update()
        {
            if (!IsOwner) return;
            
            Vector3 moveDirection = characterRigidbody.velocity;
            moveDirection.y = 0;
            bool isWalking = moveDirection.magnitude > MinWalkMagnitude;
            _isWalking.Value = isWalking;
            animator.SetBool(IsWalking, isWalking);
        }

        public override void OnNetworkSpawn()
        {
            _isWalking.OnValueChanged += (oldValue, newValue) =>
            {
                animator.SetBool(IsWalking, newValue);
            };

            wheelchair.IsGrabbed.OnValueChanged += (oldValue, newValue) =>
            {
                animator.SetBool(IsGrabbing, newValue);
            };
            
            base.OnNetworkSpawn();
        }
    }
}
