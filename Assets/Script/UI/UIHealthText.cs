using TMPro;
using UnityEngine;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private Destructible destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if (destructible != null)
            destructible.HitPointChanged -= OnHitPointChange;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;

        destructible.HitPointChanged += OnHitPointChange;
        text.text = destructible.HitPoint.ToString();
    }

    private void OnHitPointChange(float hitPoint)
    {
        text.text = hitPoint.ToString();
    }
}
