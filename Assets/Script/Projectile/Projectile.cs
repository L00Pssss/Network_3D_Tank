using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties projectileProperties;
    [SerializeField] private ProjectileMovement projectileMovement;
    [SerializeField] private ProjectileHit projectileHit;
    
    [Space(10)]
    [SerializeField] private GameObject visualModel;

    [Space(10)]
    [SerializeField] private float delayBeforeDestroy; 
    [SerializeField] private float lifeTime;
    public NetworkIdentity Owner { get; set; } // Player
    public ProjectileProperties Properties => projectileProperties;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {

        projectileHit.Check();
        projectileMovement.Move();
        
        
        if (projectileHit.IsHit == true)
            OnHit();
    }

    private void OnHit()
    {
        transform.position = projectileHit.RaycastHit.point;

        ProjectileHitResult hitResult = projectileHit.GetHitResult();

        if (hitResult.type == ProjectileHitType.Penetration)
        {
            if (NetworkSessionManager.Instance.IsServer)
            {
                SvTakeDamage();

                SvAddFrags();
            }
        }

        Destroy();
    }

    #region Server

    private void SvTakeDamage()
    {
        float damage = Properties.Damage;
        projectileHit.HitArmor.Destructible.SvApplyDamage(damage);
    }

    private void SvAddFrags()
    {
        if (projectileHit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                Player player = Owner.GetComponent<Player>();

                if (player != null)
                {
                    player.Frags++;
                }
            }
        }
    }

    #endregion


    private void Destroy()
    {
        visualModel.SetActive(false);
        enabled = false;
        
        Destroy(gameObject, delayBeforeDestroy);
    }

    public void SetProperties(ProjectileProperties properties)
    {
        this.projectileProperties = properties;
    }
}
