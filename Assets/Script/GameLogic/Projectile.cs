using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject visualModel;

    [SerializeField] private float velocity;
    [SerializeField] private float lifetime;
    [SerializeField] private float mass;

    [SerializeField] private float damage;
    [Range(0f, 1f)]
    [SerializeField] private float damageScatter;

    [SerializeField] private float impactForce;

    private const float RayAdvance = 1.1f;

    public NetworkIdentity Owner { get; set; } // Player

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void Update()
    {
        UpdateProjectile();
    }
    private void UpdateProjectile()
    {
        transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * mass)).normalized;

        Vector3 step = transform.forward * velocity * Time.deltaTime;

        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, velocity * Time.deltaTime * RayAdvance))
        {
            transform.position = hit.point;

            var destructible = hit.transform.root.GetComponent<Destructible>();

            if (destructible)
            {
                // is your prohjectile ?
                // if yes send command to server 

                if (NetworkSessionManager.Instance.IsServer)
                {
                    float dmg = damage + Random.Range(-damageScatter, damageScatter) * damage;

                    destructible.SvApplyDamage((int)dmg);

                    if (destructible.HitPoint <= 0)
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
            }

            OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

            return;
        }

        transform.position += step;
    }

    private void OnProjectileLifeEnd(Collider collider, Vector3 point, Vector3 normal)
    {
        visualModel.SetActive(false);
        enabled = false;
    }

}
