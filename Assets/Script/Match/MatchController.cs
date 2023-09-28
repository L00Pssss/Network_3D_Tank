using System.Collections;
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

    [SerializeField] private MatchMemberSpawner spawner;
    [SerializeField] private float delayAfterSpawnBeforStartMatch = 0.5f;

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

        spawner.SvRespawnVehiclesAllMembers();

        StartCoroutine(StartEventMatchWithDelay(delayAfterSpawnBeforStartMatch));
    }

    private IEnumerator StartEventMatchWithDelay(float dalay)
    {
        yield return new WaitForSeconds(dalay);

        foreach (var c in matchCoditions)
        {
            c.OnServerMatchStart(this);
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
