using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Networking.Discovery
{
    public class ServerItemRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI serverNameText;
        [SerializeField] private Button joinButton;

        public void SetName(string serverName)
        {
            serverNameText.text = serverName;
        }
    }
}
