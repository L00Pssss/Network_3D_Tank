using Mirror;
using UnityEngine;
using UnityEngine.Events;

public abstract class VehicleModule : Destructible
{
    [SerializeField] protected string title;
    [SerializeField] protected Armor armor;
    [SerializeField] protected float recoveredTime;

    protected float remainingRecoveryTime;
    public float Rem => remainingRecoveryTime / recoveredTime;
    
    public event UnityAction<float> OnTimerUpdate;

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
    
    
    // Абстрактный метод для обработки уничтожения модуля
    public abstract void OnModuleDestroyed(Destructible destructible);

}