using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Serialization;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunation;
    
    [SerializeField] protected Transform launchPoint;
    
    [SerializeField] private float fireRate;
    
    [SerializeField] protected Ammunition[] ammunitions;
    public Ammunition[] Ammunitions => ammunitions;

    private float fireTimer;
    
    public float FireTimer => fireTimer;
    
    public Transform LaunchPoint => launchPoint;

    public ProjectileProperties SelectedProjectile => ammunitions[syncSelectedAmmunitionIndex].ProjectileProperties;
    public float FireTimerNormalize => fireTimer / fireRate;
    

    [SyncVar] 
    private int syncSelectedAmmunitionIndex;

    public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;
    
    public UnityAction<float> Timer;


    public void SetSelectProjectile(int index)
    {
        if(isOwned == false) return;
        
        if(index < 0 || index > ammunitions.Length) return;

        syncSelectedAmmunitionIndex = index;

        if (isClient == true)
        {
            CmdReloadAmmunation();
        }
        
        UpdateSelectedAmmunation?.Invoke(index);
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

    [Command]
    private void CmdReloadAmmunation()
    {
        fireTimer = fireRate;
    }

    [Command]
    private void CmdFire()
    {
        if (fireTimer > 0) return;
        if (ammunitions[syncSelectedAmmunitionIndex].SvDrawAmmo(1) == false) return;

        OnFire();

        fireTimer = fireRate;

        RpcFIre();
    }

    [ClientRpc]
    private void RpcFIre()
    {
        if (isServer == true) return;

        fireTimer = fireRate;

        OnFire();
    }

    protected virtual void Update()
    {
        if (fireTimer > 0)
        {
            Timer?.Invoke(FireTimerNormalize);
            fireTimer -= Time.deltaTime;
        }
    }

}
