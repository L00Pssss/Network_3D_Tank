using UnityEngine;

public enum ProjectileHitType
{
    Penetration,
    NoPenetration,
    Ricochet,
    Environment
}
public class ProjectileHitResult
{
    public ProjectileHitType type;
    public Vector3 point;
}

[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAY_ADVANCE = 1.1f;
    
    private bool isHit;
    private RaycastHit raycastHit;
    private Armor hitArmor;

    public bool IsHit => isHit;
    public Armor HitArmor => hitArmor;
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
            Armor armor = raycastHit.collider.GetComponent<Armor>();

            if (armor != null)
            {
                hitArmor = armor;
            }

            isHit = true;
        }
    }

    public ProjectileHitResult GetHitResult()
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();

        if (hitArmor == null)
        {
            hitResult.type = ProjectileHitType.Environment;
            hitResult.point = raycastHit.point;
            return hitResult;
        }

        float normalization = projectile.Properties.NormalizationAngel;

        if (projectile.Properties.Caliber > hitArmor.Thickness * 2)
        {
            normalization = (projectile.Properties.NormalizationAngel * 1.4f * projectile.Properties.Caliber) /
                            hitArmor.Thickness;
        }

        float angel = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal,
                projectile.transform.right)) - normalization;

        float reducedArmor = hitArmor.Thickness / Mathf.Cos(angel * Mathf.Deg2Rad);

        float projectilePenetration = projectile.Properties.GetSpreadArmorPenetration();
        
        // Visual angel
        Debug.DrawRay((raycastHit.point), -projectile.transform.forward, Color.red);
        Debug.DrawRay((raycastHit.point), raycastHit.normal, Color.green);
        Debug.DrawRay((raycastHit.point), projectile.transform.right, Color.yellow);
        
       

        if (angel > projectile.Properties.RicochetAngel && projectile.Properties.Caliber < hitArmor.Thickness * 3)
            hitResult.type = ProjectileHitType.Ricochet;

        else if (projectilePenetration >= reducedArmor)
            hitResult.type = ProjectileHitType.Penetration;

        else if (projectilePenetration < reducedArmor)
            hitResult.type = ProjectileHitType.NoPenetration;
        
        Debug.Log($"armor: {hitArmor.Thickness}, resucedArmor {reducedArmor},normal {normalization}, angel: {angel}, penetration: {projectilePenetration} Type: {hitResult.type} ");
        Debug.LogError("Pause");

        hitResult.point = raycastHit.point;

        return hitResult;
    }
}
