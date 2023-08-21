using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private Turret turret;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if (turret != null)
            turret.AmmoChanged -= OnAmmoChanded;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;

        turret.AmmoChanged += OnAmmoChanded;
        text.text = turret.AmmoCount.ToString();
    }

    private void OnAmmoChanded(int ammo)
    {
        text.text = ammo.ToString();
    }
}
