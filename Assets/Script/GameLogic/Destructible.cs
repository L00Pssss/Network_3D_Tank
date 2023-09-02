using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Destructible : NetworkBehaviour
{
    public event UnityAction<float> HitPointChanged;
    public event UnityAction<Destructible> Destroyed;
    public event UnityAction<Destructible> Recovered;
    
    
    [SerializeField] private float maxHitPoint;
    [SerializeField] private UnityEvent EventDestroyed;
    [SerializeField] private UnityEvent EventRecovered;
   
    [SerializeField] private float currentHitPoint; // debug
    public float MaxHitPoint => maxHitPoint;
    public float HitPoint => currentHitPoint;
    
    [SyncVar(hook =nameof(SyncHitPoint))]
    private float syncCurrentHitPoint;

    #region  Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;
    }
    [Server]
    public void SvApplyDamage(float damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            syncCurrentHitPoint = 0;

            RpcDestroy();
        }
    }

    [Server]
    protected void SvRecovery()
    {
        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;
        
        RpcRecovery();
    }

    #endregion  
    #region  Client

    private void SyncHitPoint(float oldValue, float newValue)
    {
        currentHitPoint = newValue;
        HitPointChanged?.Invoke(newValue);
    }
    
    [ClientRpc]
    private void RpcDestroy()
    {
        Destroyed?.Invoke(this);
        EventDestroyed?.Invoke();
    }

    [ClientRpc]
    private void RpcRecovery()
    {
        Recovered?.Invoke(this);
        EventRecovered?.Invoke();
    }
    #endregion
    
}
