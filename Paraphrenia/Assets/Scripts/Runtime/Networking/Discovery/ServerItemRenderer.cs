using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Networking.Discovery
{
    /// <summary>
    /// A small rendering component used to display the server information.
    /// </summary>
    public class ServerItemRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI serverNameText;
        [SerializeField] private Button joinButton;

        /// <summary>
        /// Change the name of the ServerItem, this also renders it instantly.
        /// </summary>
        /// <param name="serverName">the name to which we need to change</param>
        public void SetName(string serverName)
        {
            serverNameText.text = serverName;
        }
    }
}
