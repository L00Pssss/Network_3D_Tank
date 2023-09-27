using System;
using UnityEngine;

public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCodition
{ 
    private int red;
    private int blue;
    private int winTeamId = -1;
    public int WinTeamId => winTeamId;

    private bool triggerd;

    public bool IsTriggered => triggerd;

    public void OnServerMatchEnd(MatchController controller)
    {


    }

    public void OnServerMatchStart(MatchController controller)
    {
       Reset();

        foreach (var player in FindObjectsOfType<Player>())
        {
            if (player.ActiveVehicle != null)
            {
                player.ActiveVehicle.Destroyed += OnVehicleDestroyed;
                
                if (player.TeamId == TeamSide.teamRed)
                    red++;
                else if (player.TeamId == TeamSide.teamBlue)
                    blue++;
            }
        }
    }

    private void OnVehicleDestroyed(Destructible destructible)
    {
        Vehicle vehicle = (destructible as Vehicle);
        
        if (vehicle == null) return;
        
        var ownerPlayer = vehicle.Owner?.GetComponent<Player>();

        if (ownerPlayer == null) return;

        switch (ownerPlayer.TeamId)
        {
            case TeamSide.teamRed:
                red--;
                break;
            case TeamSide.teamBlue:
                blue--;
                break;

        }

        if (red == 0)
        {
            winTeamId = 1;
            triggerd = true;
        }
        else 
        if (blue == 0)
        {
            winTeamId = 0;
            triggerd = true;
        }
    }

    private void Reset()
    {
        red = 0;
        blue = 0;
        triggerd = false;
    }
}
