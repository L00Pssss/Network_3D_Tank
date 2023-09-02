using UnityEngine;
using UnityEngine.Serialization;

public enum ProjectileType
{
    ArmorPiercing,
    HighExplosive,
    Subcaliber
}

[CreateAssetMenu]
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private ProjectileType type;

    [Header("Common")] 
    [SerializeField] private Projectile projectilePrefab;
    
    [Header("Movement")] 
    [SerializeField] private float velocity;

    [SerializeField] private float mass;

    [SerializeField] private float impactForce;

    [Header("Damage")] 
    [SerializeField] private float damage;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float damageSpread;

    public Projectile ProjectilePrefab => projectilePrefab;
    public ProjectileType Type => type;
    public float Velocity => velocity;
    public float Mass => mass;
    public float ImpactForce => impactForce;
    public float Damage => damage;
    public float DamageSpread => damageSpread;


    public float GetSpreadDamage()
    {
        return damage * Random.Range(1 - damageSpread, 1 + damageSpread);
    }
}
