using NeworkChat;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerList : MonoBehaviour
{
    [SerializeField] private Transform localTeamPanel;
    [SerializeField] private Transform otherTeamPanel;

    [SerializeField] private UIPlayerLable playerLablePrefab;


    private List<UIPlayerLable> allplayerLables = new List<UIPlayerLable>();


    private void Start()
    {
        PlayerList.UpdatePlayerList += OnUpdatePlayerList;
        Player.ChangeFrags += OnChangeFrags;
    }

    private void OnDestroy()
    {
        PlayerList.UpdatePlayerList -= OnUpdatePlayerList;
        Player.ChangeFrags -= OnChangeFrags;
    }

    private void OnChangeFrags(int playerNEtId, int frags)
    {
        for (int i = 0; i < allplayerLables.Count; i++)
        {
            if (allplayerLables[i].NetId == playerNEtId)
            {
                allplayerLables[i].UpdateFrag(frags);
            }
        }
    }

    private void OnUpdatePlayerList(List<PlayerData> playerData)
    {
        for (int i = 0; i < localTeamPanel.childCount; i++)
        {
            Destroy(localTeamPanel.GetChild(i).gameObject);
        }
        for (int i = 0; i < otherTeamPanel.childCount; i++)
        {
            Destroy(otherTeamPanel.GetChild(i).gameObject);
        }

        allplayerLables.Clear();

        for (int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].TeamId == Player.Local.TeamId)
            {
                AddPlayeLable(playerData[i], playerLablePrefab, localTeamPanel);
            }

            if (playerData[i].TeamId != Player.Local.TeamId)
            {
                AddPlayeLable(playerData[i], playerLablePrefab, otherTeamPanel);
            }
        }
    }

    private void AddPlayeLable(PlayerData playerData, UIPlayerLable playerLable, Transform parent)
    {
        UIPlayerLable uIPlayerLable = Instantiate(playerLable);

        uIPlayerLable.transform.SetParent(parent);

        uIPlayerLable.Initialized(playerData.Id, playerData.Nickname);

        allplayerLables.Add(uIPlayerLable);
    }
}
