using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace NeworkChat
{
    public class PlayerList : NetworkBehaviour
    {
        public static PlayerList Instance;
        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private List<PlayerData> AllPlayerData = new List<PlayerData>();

        public static UnityAction<List<PlayerData>> UpdatePlayerList;

        public override void OnStartClient()
        {
            base.OnStartClient();

            AllPlayerData.Clear();
        }

        [Server]
        public void SvAddCurrentPlayer(PlayerData data)
        {
            AllPlayerData.Add(data);
            if (isServerOnly == true)
            {
                RpcUpdateUserList(AllPlayerData);
            }
        }

        [Server]
        public void SvRemoveCurrentUser(PlayerData data)
        {
            AllPlayerData.Remove(data);
            RpcUpdateUserList(AllPlayerData);
        }

        [ClientRpc]
        private void RpcUpdateUserList(List<PlayerData> userList)
        {
            // Обновляем список только на клиенте, к которому применяется это RPC.
            UpdatePlayerList?.Invoke(userList);
        }
    }
}
