using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class VehicleModule : Destructible
{
    [SerializeField] private string title;
    [SerializeField] private Armor armor;
    [SerializeField] private float recoveredTime;

    private float remainingRecoveryTime;
    public float Rem => remainingRecoveryTime / recoveredTime;
    
    public UnityAction<float> OnTimerUpdate;

    private void Awake()
    {
        armor.SetDestuctible(this);
    }

    public override void OnStartClient()
    {
    //    base.OnStartClient();
        
        Destroyed += OnModuleDestroyed;
        enabled = false;
    }
    

    private void OnDestroy()
    {
        Destroyed -= OnModuleDestroyed;
    }
    
    

    private void Update()
    {
        if (isServer == true)
        {
            remainingRecoveryTime -= Time.deltaTime;
            
            if (remainingRecoveryTime <= 0)
            {
                
                remainingRecoveryTime = 0.0f;
                
                SvRecovery();

                enabled = false;
            }
            RpcUpdateUI(Rem);
        }
    }
    
    [ClientRpc]
    private void RpcUpdateUI(float time)
    {
        OnTimerUpdate?.Invoke(time);
    }

    private void OnModuleDestroyed(Destructible destructible)
    {
        remainingRecoveryTime = recoveredTime;
        enabled = true;
    }
}