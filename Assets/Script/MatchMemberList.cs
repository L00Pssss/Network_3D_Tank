using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MatchMemberList : NetworkBehaviour
{
    public static MatchMemberList Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<MatchMemberData> AllMemberData = new List<MatchMemberData>();

    public static UnityAction<List<MatchMemberData>> UpdateList;

    public override void OnStartClient()
    {
        base.OnStartClient();

        AllMemberData.Clear();
    }

    [Server]
    public void SvAddMember(MatchMemberData memberData)
    {
        AllMemberData.Add(memberData);

        RpcClearMemberDataList();

        for (int i = 0; i < AllMemberData.Count; i++)
        {
            RpcAddMember(AllMemberData[i]);
        }
    }

    [Server]
    public void SvRemoveMember(MatchMemberData memberData)
    {
        for (int i = 0; i < AllMemberData.Count; i++)
        {
            if (AllMemberData[i].Id == memberData.Id)
            {
                AllMemberData.RemoveAt(i);
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

        AllMemberData.Clear();
    }

    [ClientRpc]
    private void RpcAddMember(MatchMemberData memberData)
    {
        //check host
        if (isServer == true && isClient == true)
        {
            UpdateList?.Invoke(AllMemberData);
            return;
        }

        AllMemberData.Add(memberData);

        UpdateList?.Invoke(AllMemberData);
    }

    [ClientRpc]
    private void RpcRemoveMember(MatchMemberData memberData)
    {
        for (int i = 0; i < AllMemberData.Count; i++)
        {
            if (AllMemberData[i].Id == memberData.Id)
            {
                AllMemberData.RemoveAt(i);
                break;
            }
        }

        UpdateList?.Invoke(AllMemberData);
    }
}
