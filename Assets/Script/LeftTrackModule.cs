using UnityEngine;

public class LeftTrackModule : VehicleModule
{
    
    public override void OnModuleDestroyed(Destructible destructible)
    {
        remainingRecoveryTime = recoveredTime; 
        enabled = true;
    }
}
