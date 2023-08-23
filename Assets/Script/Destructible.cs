using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public UnityAction<float> HitPointChange;

    public float MaxHitPoint => maxhitPoint;
    [SerializeField] private float maxhitPoint;

    [SerializeField] private GameObject destroySfx;

    [SerializeField] private UnityEvent<Destructible> destroed;
    public UnityEvent<Destructible> OnEventDeath => destroed;

    public float HitPoint => currentHitPoint;
    private float currentHitPoint;

    [SyncVar(hook =nameof(ChangeHitPoint))]
    private float syncCurrentHitPoint;

    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxhitPoint;
        currentHitPoint = maxhitPoint;
    }
    [Server]
    public void SvApplyDamage(float damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            if (destroySfx != null)
            {
               GameObject sfx =  Instantiate(destroySfx, transform.position, Quaternion.identity);

               NetworkServer.Spawn(sfx);
            }

            syncCurrentHitPoint = 0;

            RpcDestroy();
        }
    }

    [ClientRpc]
    private void RpcDestroy()
    {
        OnDestructibleDestroy();
    }

    protected virtual void OnDestructibleDestroy()
    {
        destroed?.Invoke(this);
    }


    private void ChangeHitPoint(float oldValue, float newValue)
    {
        currentHitPoint = newValue;
        HitPointChange?.Invoke(newValue);
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {

    }
}
