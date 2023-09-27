using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MatchMemberList : NetworkBehaviour
{
    public static MatchMemberList Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<MatchMemberData> allMemberData = new List<MatchMemberData>();

    public int MemberDataCount => allMemberData.Count;

    public static UnityAction<List<MatchMemberData>> UpdateList;

    public override void OnStartClient()
    {
        base.OnStartClient();

        allMemberData.Clear();
    }

    [Server]
    public void SvAddMember(MatchMemberData memberData)
    {
        allMemberData.Add(memberData);

        RpcClearMemberDataList();

        for (int i = 0; i < allMemberData.Count; i++)
        {
            RpcAddMember(allMemberData[i]);
        }
    }

    [Server]
    public void SvRemoveMember(MatchMemberData memberData)
    {
        for (int i = 0; i < allMemberData.Count; i++)
        {
            if (allMemberData[i].Id == memberData.Id)
            {
                allMemberData.RemoveAt(i);
                break;
            }
        }

        RpcRemoveMember(memberData);
    }

    [ClientRpc]
    private void RpcClearMemberDataList()
    {
        //check host
        if (isServer == true) return;

        allMemberData.Clear();
    }

    [ClientRpc]
    private void RpcAddMember(MatchMemberData memberData)
    {
        //check host
        if (isServer == true && isClient == true)
        {
            UpdateList?.Invoke(allMemberData);
            return;
        }

        allMemberData.Add(memberData);

        UpdateList?.Invoke(allMemberData);
    }

    [ClientRpc]
    private void RpcRemoveMember(MatchMemberData memberData)
    {
        for (int i = 0; i < allMemberData.Count; i++)
        {
            if (allMemberData[i].Id == memberData.Id)
            {
                allMemberData.RemoveAt(i);
                break;
            }
        }

        UpdateList?.Invoke(allMemberData);
    }
}
