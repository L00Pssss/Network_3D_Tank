using Mirror;
using UnityEngine;

public class MatchTimer : NetworkBehaviour, IMatchCodition
{
    [SerializeField] private float matchTime;

    [SyncVar]
    private float timeLeft;
    public float TimeLeft => timeLeft;

    private bool timerEnd = false;

    public bool IsTriggered => timerEnd;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    private void Start()
    {
        if (isServer == true)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (isServer == true)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;

                timerEnd = true;
            }
        }
    }


    private void Reset()
    {
        enabled = true;
        timeLeft = matchTime;
        timerEnd = false;
    }
}
