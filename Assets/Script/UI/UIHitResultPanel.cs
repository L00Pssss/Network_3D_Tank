using UnityEngine;

public class UIHitResultPanel : MonoBehaviour
{
    [SerializeField] private Transform spawnPanel;
    [SerializeField] private UIHitResultPopup hitResultPopup;
    
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            Player.Local.ProjectileHit -= OnProjectileHit;
        }
    }

    private void OnMatchStart()
    {
        Player.Local.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileHitResult hitResult)
    {
        if(hitResult.Type == ProjectileHitType.Environment || hitResult.Type == ProjectileHitType.ModulePenetration ||
           hitResult.Type == ProjectileHitType.ModuleNoPenetration) return;

        UIHitResultPopup hitPopup = Instantiate(hitResultPopup);
        hitPopup.transform.SetParent(spawnPanel);
        hitPopup.transform.localPosition = Vector3.one;
        hitPopup.transform.position = Camera.main.WorldToScreenPoint(hitResult.Point);
        
        if(hitResult.Type == ProjectileHitType.Ricochet)
            hitPopup.SetTypeResult("Ricochet !");
        
        if(hitResult.Type == ProjectileHitType.NoPenetration)
            hitPopup.SetTypeResult("No Penetration !");
        
        hitPopup.SetDamgeResult(hitResult.Damage);
    }
}
