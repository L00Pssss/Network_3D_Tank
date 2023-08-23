using System;
using UnityEngine;

public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCodition
{ 
    private int green;
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
            if (player.ActiveVechicle != null)
            {
                player.ActiveVechicle.OnEventDeath.AddListener(OnEventDeathHandeler);

                if (player.TeamId == TeamSide.teamGreen)
                    green++;
                else if (player.TeamId == TeamSide.teamBlue)
                    blue++;
            }
        }
    }

    private void OnEventDeathHandeler(Destructible destructible)
    {
        var ownerPlayer = destructible.Owner?.GetComponent<Player>();

        if (ownerPlayer != null) return;

        switch (ownerPlayer.TeamId)
        {
            case TeamSide.teamGreen:
                green--;
                break;
            case TeamSide.teamBlue:
                blue--;
                break;

        }

        if (green == 0)
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
        green = 0;
        blue = 0;
        triggerd = false;
    }
}
