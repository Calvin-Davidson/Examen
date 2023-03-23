using System;
using System.Linq;
using System.Text.RegularExpressions;
using Netcode.Transports.PhotonRealtime;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Runtime.Networking.Discovery
{
    /// <summary>
    /// used to render the NetworkDiscovery data, and handles UI input to start our own server or update's the list.
    /// </summary>
    public class NetworkDiscoveryRenderer : MonoBehaviour
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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newID = new string(Enumerable.Repeat(chars, 5).Select(s => s[r.Next(s.Length)]).ToArray());

            displayText.text = serverStartingDisplayText;
            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.OnServerStarted += () =>
            {
                displayText.text = waitingForOtherPlayerDisplayText.Replace("%roomName%", newID);
                onServerStarted?.Invoke();
            };
        }

        private void HandleJoinServer()
        {
            onStartConnection?.Invoke();
            _transport.RoomName = serverIDInputField.text;

            // Checks if the input roomName is of a valid length and does not contain any special characters.
            if (serverIDInputField.text.Length != 5 || !Regex.IsMatch(displayText.text, "/^[A-Za-z0-9]*$/"))
            {
                onInvalidKey?.Invoke();
                return;
            }

            onServerJoined?.Invoke();
            displayText.text = joiningServerDisplayText.Replace("%roomName%", _transport.RoomName);
        }
    }
}