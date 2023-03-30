using Unity.Netcode;

namespace Runtime.Misc
{
    /// <summary>
    /// Some object such as game controls should only be visible for the server.
    /// This helper script disabled this gameObject on non-server clients.
    /// </summary>
    public class ServerOnlyObject : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            gameObject.SetActive(IsServer);
        }
    }
}
