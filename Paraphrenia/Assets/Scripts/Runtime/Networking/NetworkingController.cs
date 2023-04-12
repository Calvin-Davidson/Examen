using System;
using Toolbox.Utilities;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Utils;
using VivoxUnity;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;

namespace Runtime.Networking
{
    public class NetworkingController : NetworkBehaviourSingleton<NetworkingController>
    {
        [SerializeField, Range(1, 2)] private int requiredPlayers = 2;
        [SerializeField] private KeyCode forceStartKey = KeyCode.P;
        [SerializeField] private string targetScene;

#if (UNITY_EDITOR)
        [SerializeField] private UnityEditor.SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null) targetScene = sceneAsset.name;
        }
#endif

        public UnityEvent onPlayerJoin = new();
        public UnityEvent onPlayerLeave = new();

        private readonly NetworkVariable<bool> _isServerNurse = new();
        private ILoginSession _loginSession;
        private IChannelSession _channelSession;
        private string _roomCode;

        public string RoomCode
        {
            get => _roomCode;
            set => _roomCode = value;
        }
        private new async void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandlePlayerJoin;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandlePlayerDisconnect;
            DontDestroyOnLoad(this);
            
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            VivoxService.Instance.Initialize();
            
            Login(Random.Range(0, int.MaxValue).ToString());
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(forceStartKey) &&
                (!NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
            {
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            }
        }

        public override void OnNetworkSpawn()
        {
            JoinChannel(RoomCode, ChannelType.Echo, true, false);
            
            _channelSession.Participants.AfterKeyAdded += (sender, arg) =>
            {
                Debug.Log("AfterKeyAdded");
            };
            _channelSession.Participants.BeforeKeyRemoved += (sender, arg) =>
            {
                Debug.Log("BeforeKeyRemoved");
            };
            _channelSession.Participants.AfterValueUpdated += (sender, arg) =>
            {
                Debug.Log("AfterValueUpdated");
            };
            
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            LogOut();
            base.OnNetworkDespawn();
        }

        private void HandlePlayerJoin(ulong id)
        {
            onPlayerJoin?.Invoke();
            
            if (!NetworkManager.Singleton.IsServer) return;

            if (NetworkManager.Singleton.ConnectedClients.Count == requiredPlayers)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            }
        }

        private void HandlePlayerDisconnect(ulong id)
        {
            onPlayerLeave?.Invoke();
        }

        public void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText,
            bool transmissionSwitch = true, Channel3DProperties properties = null)
        {
            if (_loginSession.State == LoginState.LoggedIn)
            {
                Channel channel = new Channel(channelName, channelType, properties);
                _channelSession = _loginSession.GetChannelSession(channel);

                _channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch,
                    _channelSession.GetConnectToken(), ar =>
                    {
                        try
                        {
                            _channelSession.EndConnect(ar);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Could not connect to channel: {e.Message}");
                            return;
                        }
                    });
            }
            else
            {
                Debug.LogError("Can't join a channel when not logged in.");
            }
        }

        public void Login(string displayName = null)
        {
            var account = new Account(displayName);
            bool connectAudio = true;
            bool connectText = true;

            _loginSession = VivoxService.Instance.Client.GetLoginSession(account);
            _loginSession.PropertyChanged += LoginSession_PropertyChanged;

            _loginSession.BeginLogin(_loginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
            {
                try
                {
                    _loginSession.EndLogin(ar);
                }
                catch (Exception e)
                {
                    // Unbind any login session-related events you might be subscribed to.
                    // Handle error
                    return;
                }
                // At this point, we have successfully requested to login. 
                // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
                // Reference LoginSession_PropertyChanged()
            });
        }

// For this example, we immediately join a channel after LoginState changes to LoginState.LoggedIn.
// In an actual game, when to join a channel will vary by implementation.
        private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var loginSession = (ILoginSession)sender;
            if (e.PropertyName == "State")
            {
                if (loginSession.State == LoginState.LoggedIn)
                {
                    bool connectAudio = true;
                    bool connectText = true;

                    // This puts you into an echo channel where you can hear yourself speaking.
                    // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                    JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                    // To test with multiple users, try joining a non-positional channel.
                    // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
                }
            }
        }

        void LogOut()
        {
            // For this example, _loginSession is assumed to be an ILoginSession.
            _loginSession.Logout();
            VivoxService.Instance.Client.Uninitialize();
        }
        
        public NetworkVariable<bool> IsServerNurse
        {
            get => _isServerNurse;
        }
    }
}