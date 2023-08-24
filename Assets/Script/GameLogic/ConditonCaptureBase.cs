using UnityEngine;
using Mirror;

public class ConditonCaptureBase : NetworkBehaviour, IMatchCodition
{
    [SerializeField] private TeamBase redBase;
    [SerializeField] private TeamBase blueBase;

    [SyncVar]
    private float redBaseCaptureLevel;
    public float RedBaseCaptureLevel => redBaseCaptureLevel;

    [SyncVar]
    private float blueBaseCaptureLevel;
    public float BlueBaseCaptureLevel => blueBaseCaptureLevel;

    private bool triggered;

    public bool IsTriggered => triggered;

    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    private void Start()
    {
        enabled = true;
    }

    private void Update()
    {
        if (isServer == true)
        {
            redBaseCaptureLevel = redBase.CaptureLevel;
            blueBaseCaptureLevel = blueBase.CaptureLevel;

            if (redBaseCaptureLevel == 100 || blueBaseCaptureLevel == 100)
            {
                triggered = true;
            }
        }
    }

    private void Reset()
    {
        redBase.Reset();
        blueBase.Reset();

        blueBaseCaptureLevel = 0;
        redBaseCaptureLevel = 0;

        triggered = false;
        enabled = true;
    }
}
