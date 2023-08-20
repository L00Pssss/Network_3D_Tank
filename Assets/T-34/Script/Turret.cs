using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Turret : NetworkBehaviour
{
    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;

    [SerializeField] private float fireRate;

    [SerializeField] protected Projectile projectile;
    public Projectile Projectile => projectile;

    private float fireTimer;
    public float FireTimerNormalize => fireTimer / fireRate;

    [SyncVar]
    [SerializeField] protected int ammoCount;
    public int AmmoCount => ammoCount;


    public UnityAction<int> AmmoChanged;


    [Server]
    public void SvAddAmmo(int count)
    {
        ammoCount += count;
        RpcAmmoChanged();
    }

    [Server]
    protected virtual bool SvDrawAmmo(int count)
    {
        if (ammoCount == 0)
        {
            return false;
        }
        if (ammoCount >= count)
        {
            ammoCount -= count;

            RpcAmmoChanged();
            return true;
        }

        return false;
    }

    [ClientRpc]
    protected void RpcAmmoChanged()
    {
        AmmoChanged?.Invoke(ammoCount);
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
    private void CmdFire()
    {
        if (fireTimer > 0) return;
        if (SvDrawAmmo(1) == false) return;

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
            fireTimer -= Time.deltaTime;
        }
    }

}
