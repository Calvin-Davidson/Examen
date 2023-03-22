using Unity.Netcode;

namespace Runtime.Networking.Discovery
{
    /// <summary>
    /// The data the server sends back to the client
    /// </summary>
    public struct DiscoveryResponseData: INetworkSerializable
    {
        public ushort Port;

        public string ServerName;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Port);
            serializer.SerializeValue(ref ServerName);
        }
    }
}