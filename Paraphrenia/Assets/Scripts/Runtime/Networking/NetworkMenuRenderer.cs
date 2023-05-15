using System.Linq;
using System.Text.RegularExpressions;
using Netcode.Transports.PhotonRealtime;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.Networking
{
    /// <summary>
    /// used to render the NetworkDiscovery data, and handles UI input to start our own server or update's the list.
    /// </summary>
    public class NetworkMenuRenderer : MonoBehaviour
    {
        [SerializeField] private Button startServerButton;
        [SerializeField] private Button joinServerButton;

        [SerializeField] private TMP_InputField serverIDInputField;
        [SerializeField] private TextMeshProUGUI displayText;

        [SerializeField, TextArea] private string serverStartingDisplayText;

        [SerializeField, TextArea, Tooltip("The text displayed when starting a server")]
        private string joiningServerDisplayText;

        [SerializeField, TextArea, Tooltip("The text displayed when waiting for other players")]
        private string waitingForOtherPlayerDisplayText;


        private PhotonRealtimeTransport _transport;

        public UnityEvent onStartConnection = new UnityEvent();
        public UnityEvent onServerStarted = new UnityEvent();
        public UnityEvent onServerJoined = new UnityEvent();

        public UnityEvent onInvalidKey = new UnityEvent();

        /// <summary>
        /// Fetches the required components and subscribes to there events.
        /// </summary>
        private void Awake()
        {
            _transport = FindObjectOfType<PhotonRealtimeTransport>();

            startServerButton.onClick.AddListener(HandleStartServer);
            joinServerButton.onClick.AddListener(HandleJoinServer);
        }


        private void HandleStartServer()
        {
            onStartConnection?.Invoke();
            System.Random r = new System.Random();
            const string chars = "0123456789";
            string newID = new string(Enumerable.Repeat(chars, 5).Select(s => s[r.Next(s.Length)]).ToArray());
            _transport.RoomName = newID;
            
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                displayText.text = waitingForOtherPlayerDisplayText.Replace("%roomName%", _transport.RoomName);
                onServerStarted?.Invoke();
            };
            
            displayText.text = serverStartingDisplayText;
            NetworkManager.Singleton.StartHost();
        }

        private void HandleJoinServer()
        {
            onStartConnection?.Invoke();
            _transport.RoomName = serverIDInputField.text;

            // Checks if the input roomName is of a valid length and does not contain any special characters.
            if (serverIDInputField.text.Length != 5 || Regex.IsMatch(serverIDInputField.text, "[^A-Za-z0-9]+")) 
            { 
                onInvalidKey?.Invoke();
                return;
            }

            NetworkManager.Singleton.StartClient();
            onServerJoined?.Invoke();
            displayText.text = joiningServerDisplayText.Replace("%roomName%", _transport.RoomName);
        }
    }
}