using Mirror;
using UnityEngine.Events;
using UnityEngine;

public interface IMatchCodition
{
    bool IsTriggered { get; }
    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}


public class MatchController : NetworkBehaviour
{
    private static int TeamIdCounter;

    public static int GetNextTeamID()
    {
        return TeamIdCounter++ % 2;
    }
    public static void ResetTeamCounter()
    {
        TeamIdCounter = 1;
    }

    public event UnityAction MatchStart;
    public event UnityAction MatchEnd;

    public event UnityAction SvMatchStart;
    public event UnityAction SvMatchEnd;

    [SyncVar]
    private bool matchActive;
    public bool IsMatchActive => matchActive;


    public int WinTeamId = -1;

    private IMatchCodition[] matchCoditions;

    private void Awake()
    {
        matchCoditions = GetComponentsInChildren<IMatchCodition>();
    }

    private void Update()
    {
        if (isServer == true)
        {
            if (matchActive == true)
            {
                foreach (var match in matchCoditions)
                {
                    if (match.IsTriggered == true)
                    {
                        SvEndMatch();
                        break;
                    }
                }
            }
        }
    }

    [Server]
    public void SvRestartMatch()
    {
        if (matchActive == true) return;

        matchActive = true;

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

        foreach (var v in matchCoditions)
        {
            v.OnServerMatchStart(this);
        }

        SvMatchStart?.Invoke();

        RpcMatchStart();
    }

    [Server]
    public void SvEndMatch()
    {
        foreach (var v in matchCoditions)
        {
            v.OnServerMatchEnd(this);

            if (v is ConditionTeamDeathmatch)
            {
                WinTeamId = (v as ConditionTeamDeathmatch).WinTeamId;
            }

            if (v is ConditonCaptureBase)
            {
                if ((v as ConditonCaptureBase).RedBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.teamBlue;
                } 
                if ((v as ConditonCaptureBase).BlueBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.teamRed;
                }
            }
        }

        matchActive = false;

        SvMatchEnd?.Invoke();

        RpcMatchEnd(WinTeamId);
    }

    [ClientRpc]
    private void RpcMatchStart()
    {
        MatchStart?.Invoke();
    }

    [ClientRpc]
    private void RpcMatchEnd(int winTeamId)
    {
        WinTeamId = winTeamId;
        MatchEnd?.Invoke();
    }
}
