using Mirror;
using UnityEngine.Events;

public interface IMatchCodition
{
    bool IsTriggered { get; }
    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}


public class MatchController : NetworkBehaviour
{
    public UnityAction MatchStart;
    public UnityAction MatchEnd;

    public UnityAction SvMatchStart;
    public UnityAction SvMatchEnd;

    [SyncVar]
    private bool matchActive;
    public bool IsMatchActive => matchActive;

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
            if (v.ActiveVechicle != null)
            {
                NetworkServer.UnSpawn(v.ActiveVechicle.gameObject);
                Destroy(v.ActiveVechicle.gameObject);

                v.ActiveVechicle = null;
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
        }

        matchActive = false;

        SvMatchEnd?.Invoke();

        RpcMatchEnd();
    }

    [ClientRpc]
    private void RpcMatchStart()
    {

        MatchStart?.Invoke();
    }

    [ClientRpc]
    private void RpcMatchEnd()
    {
        MatchEnd?.Invoke();
    }
}
