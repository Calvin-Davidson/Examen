using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Networking.Discovery
{
    [DisallowMultipleComponent]
    public abstract class NetworkDiscovery<TBroadCast, TResponse> : MonoBehaviour where TBroadCast : INetworkSerializable, new() where TResponse : INetworkSerializable, new()
    {
        /// <summary>
        /// used the differentiate between the client and server messages.
        /// </summary>
        private enum MessageType : byte
        {
            BroadCast = 0,
            Response = 1,
        }
        
        /// <summary>
        /// The discovery server's port, the client will broadcast to this port.
        /// </summary>
        [SerializeField] private ushort port = 47777;

        // This is long because unity inspector does not like ulong.
        [SerializeField, Tooltip("This is the uniqueID of our app, and ensures we don't get mixed up with other apps")]
        private long uniqueApplicationId;

        
        /// <summary>
        /// The UdpClient reference, can be null when it's the discovery has not started yet.
        /// </summary>
        private UdpClient _client;

        
        /// <summary>
        /// Gets a value indicating whether the discovery is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets whether the discovery is in server mode.
        /// </summary>
        public bool IsServer { get; private set; }

        /// <summary>
        /// Gets whether the discovery is in client mode.
        /// </summary>
        public bool IsClient { get; private set; }

        /// <summary>
        /// We should stop the discovery when the applications quits. So we can close the UDP connection
        /// </summary>
        public void OnApplicationQuit()
        {
            StopDiscovery();
        }

        /// <summary>
        /// Editor only function that handles changes to the script and makes sure this network discovery has a unique ID.
        /// This is so we don't listen to invalid or other broadcasting services.
        /// </summary>
        void OnValidate()
        {
            if (uniqueApplicationId == 0)
            {
                var value1 = (long) Random.Range(int.MinValue, int.MaxValue);
                var value2 = (long) Random.Range(int.MinValue, int.MaxValue);
                uniqueApplicationId = value1 + (value2 << 32);
            }
        }

    
        /// <summary>
        /// Broadcasts a message to let the servers know the client is looking for servers
        /// </summary>
        /// <param name="broadCast">The broadcast object</param>
        /// <exception cref="InvalidOperationException">Thrown when the discovery is not in client mode</exception>
        public void ClientBroadcast(TBroadCast broadCast)
        {
            if (!IsClient)
            {
                throw new InvalidOperationException("Cannot send client broadcast while not running in client mode. Call StartClient first.");
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, port);

            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp, 1024 * 64))
            {
            
                WriteHeader(writer, MessageType.BroadCast);

                writer.WriteNetworkSerializable(broadCast);
                var data = writer.ToArray();

                try
                {
                    Debug.Log("[NetCodeDiscovery] Sending broadcast message");
                    // This works because PooledBitStream.Get resets the position to 0 so the array segment will always start from 0.
                    _client.SendAsync(data, data.Length, endPoint);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// Starts the discovery in server mode which will respond to client broadcasts searching for servers.
        /// </summary>
        public void StartServer()
        {
            StartDiscovery(true);
        }

        /// <summary>
        /// Starts the discovery in client mode. <see cref="ClientBroadcast"/> can be called to send out broadcasts to servers and the client will actively listen for responses.
        /// </summary>
        public void StartClient()
        {
            StartDiscovery(false);
        }
    
    
        /// <summary>
        /// Stops the discovery and closes the client
        /// </summary>
        public void StopDiscovery()
        {
            IsClient = false;
            IsServer = false;
            IsRunning = false;

            if (_client != null)
            {
                try
                {
                    _client.Close();
                }
                catch (Exception)
                {
                    // We don't care about socket exception here. Socket will always be closed after this.
                }

                _client.Dispose();
                _client = null;
            }
        }

        /// <summary>
        /// Gets called whenever a broadcast is received. Creates a response based on the incoming broadcast data.
        /// </summary>
        /// <param name="sender">The sender of the broadcast</param>
        /// <param name="broadCast">The broadcast data which was sent</param>
        /// <param name="response">The response to send back</param>
        /// <returns>True if a response should be sent back else false</returns>
        protected abstract bool ProcessBroadcast(IPEndPoint sender, TBroadCast broadCast, out TResponse response);

        /// <summary>
        /// Gets called when a response to a broadcast gets received
        /// </summary>
        /// <param name="sender">The sender of the response</param>
        /// <param name="response">The value of the response</param>
        protected abstract void ResponseReceived(IPEndPoint sender, TResponse response);

        void StartDiscovery(bool isServer)
        {
            StopDiscovery();

            IsServer = isServer;
            IsClient = !isServer;
            IsRunning = true;
        
            // If we are not a server we use the 0 port (let udp client assign a free port to us)
            var selectedPort = isServer ? this.port : 0;

            _client = new UdpClient(selectedPort) {EnableBroadcast = true, MulticastLoopback = false};
            Debug.Log("[NetCodeDiscovery] Started a UDP client");

            _ = ListenAsync(isServer ? ReceiveBroadcastAsync : new Func<Task>(ReceiveResponseAsync));
        }

        /// <summary>
        /// The async loop, this is used to call the Listeners for both the server and client and once we receive a
        /// message it will simply re-run so we get get all messages.
        /// </summary>
        /// <param name="onReceiveTask"></param>
        async Task ListenAsync(Func<Task> onReceiveTask)
        {
            while (IsRunning)
            {
                try
                {
                    await onReceiveTask();
                }
                catch (ObjectDisposedException)
                {
                    // socket has been closed
                    break;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// is the listener for messages from the server. once we receive a broadcast message it will check if it's from the server.
        /// and if so it will call the {ResponseReceived} methode.
        /// </summary>
        async Task ReceiveResponseAsync()
        {
            UdpReceiveResult udpReceiveResult = await _client.ReceiveAsync();
        
            if (IsServer) return;

            var segment = new ArraySegment<byte>(udpReceiveResult.Buffer, 0, udpReceiveResult.Buffer.Length);
            using var reader = new FastBufferReader(segment, Allocator.Temp);

            try
            {
                if (ReadAndCheckHeader(reader, MessageType.Response) == false)
                {
                    return;
                }
            
                reader.ReadNetworkSerializable(out TResponse receivedResponse);
                ResponseReceived(udpReceiveResult.RemoteEndPoint, receivedResponse);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// The listener on the server. will listen for broadcast messages from the client and if it receives
        /// a message it will send it's own data back so the client knows this is a valid server which it can join.
        /// </summary>
        async Task ReceiveBroadcastAsync()
        {
            Debug.Log("[NetCodeDiscovery] Starting to listen for clients");
            UdpReceiveResult udpReceiveResult = await _client.ReceiveAsync();

            if (IsClient) return;
        
            var segment = new ArraySegment<byte>(udpReceiveResult.Buffer, 0, udpReceiveResult.Buffer.Length);
            using var reader = new FastBufferReader(segment, Allocator.Temp);

            try
            {
                if (ReadAndCheckHeader(reader, MessageType.BroadCast) == false)
                {
                    Debug.Log("[NetCodeDiscovery] Application ID and message type are not what we expect");
                    return;
                }
            
                reader.ReadNetworkSerializable(out TBroadCast receivedBroadcast);

                if (ProcessBroadcast(udpReceiveResult.RemoteEndPoint, receivedBroadcast, out TResponse response))
                {
                    using var writer = new FastBufferWriter(1024, Allocator.Temp, 1024 * 64);
                    WriteHeader(writer, MessageType.Response);

                    writer.WriteNetworkSerializable(response);
                    var data = writer.ToArray();

                    Debug.Log("[NetCodeDiscovery] Sending response to client");
                    await _client.SendAsync(data, data.Length, udpReceiveResult.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    
        /// <summary>
        /// Writes the default header that both the client and server messages require.
        /// </summary>
        /// <param name="writer">The writer to which we write the data</param>
        /// <param name="messageType">The type of message we are sending.</param>
        private void WriteHeader(FastBufferWriter writer, MessageType messageType)
        {
            // Serialize unique application id to make sure packet received is from same application.
            writer.WriteValueSafe(uniqueApplicationId);

            // Write a flag indicating whether this is a broadcast
            writer.WriteByteSafe((byte) messageType);
        }

        /// <summary>
        /// Check if the packet is send by our application and makes sure it's the type we expect it to be.
        /// </summary>
        /// <param name="reader">The buffer from which we read the packet data</param>
        /// <param name="expectedType">The type we expect this packet to be.</param>
        /// <returns></returns>
        private bool ReadAndCheckHeader(FastBufferReader reader, MessageType expectedType)
        {
            reader.ReadValueSafe(out long receivedApplicationId);
            if (receivedApplicationId != uniqueApplicationId)
            {
                return false;
            }

            reader.ReadByteSafe(out byte messageType);
            if (messageType != (byte) expectedType)
            {
                return false;
            }

            return true;
        }
    }
}