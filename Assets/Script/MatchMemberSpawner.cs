using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchMemberSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject botPrefab;
    [Range(0, 15)]
    [SerializeField] private int targetAmountMemberTeam;

    [Server]
    public void SvRespawnVehiclesAllMembers()
    {
        SvRespawnPlayerVehicle();
        SvRespawnBotVehicle();
    }

    [Server]
    private void SvRespawnPlayerVehicle()
    {
        foreach (var v in FindObjectsOfType<Player>())
        {
            if (v.ActiveVehicle != null)
            {
                NetworkServer.UnSpawn(v.ActiveVehicle.gameObject);
                Destroy(v.ActiveVehicle.gameObject);

                v.ActiveVehicle = null;
            }
        }

        foreach (var v in FindObjectsOfType<Player>())
        {
            v.SvSpwanClintVehicle();
        }
    }

    [Server]
    private void SvRespawnBotVehicle()
    {
        foreach (var bot in FindObjectsOfType<Bot>())
        {
            NetworkServer.UnSpawn(bot.gameObject);
            Destroy(bot.gameObject);
        }

        int botAmount = targetAmountMemberTeam * 2 - MatchMemberList.Instance.MemberDataCount;

        for (int i = 0; i < botAmount; i++)
        {
            GameObject bot = Instantiate(botPrefab);
            NetworkServer.Spawn(bot);
        }
    }
}
