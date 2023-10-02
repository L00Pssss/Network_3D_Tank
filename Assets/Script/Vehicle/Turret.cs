using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunation; 
    public event UnityAction Shot;
    
    [SerializeField] protected Transform launchPoint;
    
    [SerializeField] private float fireRate;
    
    [SerializeField] protected Ammunition[] ammunitions;
    public Ammunition[] Ammunitions => ammunitions;

    private float fireTimer;
    
    public Transform LaunchPoint => launchPoint;

    public ProjectileProperties SelectedProjectile => Ammunitions[syncSelectedAmmunitionIndex].ProjectileProperties;
    public float FireTimerNormalize => fireTimer / fireRate;
    

    [SyncVar] 
    [SerializeField] private int syncSelectedAmmunitionIndex;

    public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;
    
    public UnityAction<float> Timer;


    public void SetSelectProjectile(int index)
    {
        if(isOwned == false) return;
        
        if(index < 0 || index > ammunitions.Length) return;

        syncSelectedAmmunitionIndex = index;

        if (isClient == true)
        {
            CmdSelectProjectile(index);
            CmdReloadAmmunation();
        }
        
        UpdateSelectedAmmunation.Invoke(syncSelectedAmmunitionIndex);
    }
    

    protected virtual void OnFire() { }

    public void Fire()
    {
        if (isOwned == false) return;

        if (isClient == true)
        {
            CmdFire();
        }
    }

    [Server]
    public void SvFire()
    {
        if (fireTimer > 0) return;
        if (ammunitions[syncSelectedAmmunitionIndex].SvDrawAmmo(1) == false) return;

        OnFire();

        fireTimer = fireRate;

        RpcFIre();
        
        Shot?.Invoke();
    }

    [Command]
    private void CmdReloadAmmunation()
    {
        fireTimer = fireRate;
    }
    
    [Command]
    private void CmdSelectProjectile(int index)
    {
        // Обновление выбранного боеприпаса на сервере
        syncSelectedAmmunitionIndex = index;
    }

    [Command]
    private void CmdFire()
    {
        SvFire();
    }

    [ClientRpc]
    private void RpcFIre()
    {
        if (isServer == true) return;

        fireTimer = fireRate;

        OnFire();
        
        Shot?.Invoke();
    }
    
    [ClientRpc]
    private void RpcUpdateTimer(float normalizedTimer)
    {
        Timer?.Invoke(normalizedTimer);
    }

    protected virtual void Update()
    {
        if (fireTimer > 0)
        {
            if (isServer)
            {
                RpcUpdateTimer(FireTimerNormalize);
            }
            fireTimer -= Time.deltaTime;
        }
    }

}
