using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public UnityAction<float> HitPointChange;

    public float MaxHitPoint => m_MaxhitPoint;
    [SerializeField] 
    private float m_MaxhitPoint;

    [SerializeField] 
    private GameObject m_DestroySfx;

    public float HitPoint => currentHitPoint;
    private float currentHitPoint;

    [SyncVar(hook =nameof(ChangeHitPoint))]
    private float syncCurrentHitPoint;

    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = m_MaxhitPoint;
        currentHitPoint = m_MaxhitPoint;
    }
    [Server]
    public void SvApplyDamage(float damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            if (m_DestroySfx != null)
            {
               GameObject sfx =  Instantiate(m_DestroySfx, transform.position, Quaternion.identity);

               NetworkServer.Spawn(sfx);
            }

            Destroy(gameObject);
        }
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
