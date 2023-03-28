using System.Linq;
using Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.Misc
{
    public static class PlayerTypeExtensions
    {
        /// <summary>
        /// Returns the networkClient ID based on which type you request,
        /// make it easier to differentiate between the nurse player and the wheelchair player.
        /// </summary>
        /// <param name="type">The playerType from which we would like the ID.</param>
        /// <returns>the clientID of the networkClient</returns>
        public static ulong GetNetworkClientID(this PlayerType type)
        {
            if (type == PlayerType.Both || type == PlayerType.None)
            {
                Debug.Log($"[PlayerType] Cannot return id of playerType: {type}");
                return ulong.MaxValue;
            }

            NetworkManager networkManager = NetworkManager.Singleton;
            
            ulong serverClientID = NetworkManager.ServerClientId;
            ulong localClientID = networkManager.LocalClientId;
            ulong nonServerID = networkManager.IsServer ? networkManager.ConnectedClientsIds.FirstOrDefault(id => id != localClientID) : localClientID;
            
            bool isServerNurse = NetworkingController.Instance.IsServerNurse();

            if (type == PlayerType.Nurse)
            {
                return (isServerNurse ? serverClientID : nonServerID);
            }

            return isServerNurse ? nonServerID : serverClientID;
        }
    }

    public enum PlayerType
    {
        Nurse,
        Wheelchair,
        Both,
        None
    }
}
