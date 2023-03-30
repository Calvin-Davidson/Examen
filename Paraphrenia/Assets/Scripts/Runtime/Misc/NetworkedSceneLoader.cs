using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Misc
{
    public class NetworkedSceneLoader : NetworkBehaviour
    {
        [SerializeField] private string targetScene;
        [SerializeField] private LoadSceneMode loadSceneMode;
#if (UNITY_EDITOR)
        [SerializeField] private UnityEditor.SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null) targetScene = sceneAsset.name;
        }
#endif

        public void LoadScene()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene, loadSceneMode);
        }
    }
}