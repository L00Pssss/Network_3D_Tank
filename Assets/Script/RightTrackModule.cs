using UnityEngine;

public class RightTrackModule : VehicleModule
{
    public override void OnModuleDestroyed(Destructible destructible)
    {
        remainingRecoveryTime = recoveredTime; 
        enabled = true;
    }
}
