using Unity.Netcode;

namespace Runtime.Networking.Discovery
{
    /// <summary>
    /// The data we send to the server
    /// </summary>
    public struct DiscoveryBroadcastData : INetworkSerializable
    {
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        }
    }
}