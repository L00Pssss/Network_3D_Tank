using UnityEngine;
[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAY_ADVANCE = 1.1f;
    
    private Destructible hitDestructible;
    private bool isHit;
    private RaycastHit raycastHit;

    public bool IsHit => isHit;
    public Destructible HitDestructible => hitDestructible;
    public RaycastHit RaycastHit => raycastHit;
    
    private Projectile projectile;
    
    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    public void Check()
    {
        if (isHit == true) return;
        

        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
        {
            var destructible = raycastHit.transform.root.GetComponent<Destructible>();

            if (destructible != null)
            {
                hitDestructible = destructible;
            }

            isHit = true;
        }
    }
}
