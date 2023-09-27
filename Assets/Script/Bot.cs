using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bot : MatchMember
{

    [SerializeField] private Vehicle vehicle;
        
    private List<string> availableNames = new List<string>();
    public override void OnStartServer()
    {
        base.OnStartServer();

        teamId = MatchController.GetNextTeamID();
        nickname = "b_" + GetRandomName();

        data = new MatchMemberData((int)netId, nickname, teamId, netIdentity);

        transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(teamId);

        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = teamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
        MatchMemberList.Instance.SvRemoveMember(data);
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = teamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;
    }

    private void Start()
    {
        if (isServer == true)
        {
            MatchMemberList.Instance.SvAddMember(data);
        }
    }

    private string GetRandomName()
    {
        
        if (availableNames.Count == 0)
        {
            FillAvailableNames();
        }

        int randomIndex = Random.Range(0, availableNames.Count);
        string randomName = availableNames[randomIndex];
        availableNames.RemoveAt(randomIndex); // Удаляем имя из списка, чтобы оно больше не повторялось
        return randomName;
        
    }
    
    private void FillAvailableNames()
    {
        availableNames.AddRange(new string[]
        {
            "Emily", "Jacob", "Olivia", "Ethan", "Sophia", "Liam", "Mia", "Noah", "Ava", "William", 
            "Isabella", "James", "Emma", "Benjamin", "Charlotte", "Michael", "Amelia", "Alexander", 
            "Harper", "Daniel", "Abigail", "Samuel", "Grace", "Christopher", "Lily", "Henry", "Natalie", 
            "Joshua", "Chloe"
        });
    }
}
