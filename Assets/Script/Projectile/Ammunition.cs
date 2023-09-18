using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Ammunition : NetworkBehaviour
{
    public event UnityAction<int> AmmoCountChanged;
    
    [SerializeField] private ProjectileProperties projectileProperties;

    [SyncVar(hook = nameof(SyncAmmoCount))] 
    [SerializeField] private int syncAmmoCount;
    
    public ProjectileProperties ProjectileProperties => projectileProperties;
    public int AmmCount => syncAmmoCount;

    #region  Server

    // [Server]
    // public void SvAddAmmo(int count)
    // {
    //     syncAmmoCount += count;
    // }

    [Server]
    public bool SvDrawAmmo(int count)
    {
        if (syncAmmoCount == 0)
        {
            return false;
        }
        if (syncAmmoCount >= count)
        {
            syncAmmoCount -= count;
            
            return true;
        }

        return false;
    }
    #endregion

    #region Client
    
    private void SyncAmmoCount(int oldValue, int newValue)
    {
        Debug.Log(newValue + " SyncAmmoCount");
        AmmoCountChanged?.Invoke(newValue);
    }

    #endregion

}
