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
        MatchMemberList.UpdateList += OnUpdatePlayerList;
        Player.ChangeFrags += OnChangeFrags;
    }

    private void OnDestroy()
    {
        MatchMemberList.UpdateList -= OnUpdatePlayerList;
        Player.ChangeFrags -= OnChangeFrags;
    }

    private void OnChangeFrags(MatchMember member, int frags)
    {
        for (int i = 0; i < allplayerLables.Count; i++)
        {
            if (allplayerLables[i].NetId == member.netId)
            {
                allplayerLables[i].UpdateFrag(frags);
            }
        }
    }

    private void OnUpdatePlayerList(List<MatchMemberData> memberData)
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

        for (int i = 0; i < memberData.Count; i++)
        {
            if (memberData[i].TeamId == Player.Local.TeamId)
            {
                AddPlayeLable(memberData[i], playerLablePrefab, localTeamPanel);
            }

            if (memberData[i].TeamId != Player.Local.TeamId)
            {
                AddPlayeLable(memberData[i], playerLablePrefab, otherTeamPanel);
            }
        }
    }

    private void AddPlayeLable(MatchMemberData playerData, UIPlayerLable playerLable, Transform parent)
    {
        UIPlayerLable uIPlayerLable = Instantiate(playerLable);

        uIPlayerLable.transform.SetParent(parent);

        uIPlayerLable.Initialized(playerData.Id, playerData.Nickname);

        allplayerLables.Add(uIPlayerLable);
    }
}
