using Unity.Netcode;
using UnityEngine.Events;

namespace Runtime.Puzzels
{
    public abstract class Puzzle : NetworkBehaviour
    {
        public UnityEvent onCompleteNetworked = new();

        [ClientRpc]
        public virtual void DoCompleteClientRpc()
        {
            onCompleteNetworked?.Invoke();
        }
    }
}
