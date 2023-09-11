using System;
using UnityEngine;
using UnityEngine.UI;

public class UIReloadModule : MonoBehaviour
{
    [SerializeField] private Slider reloadModule;
    

    [SerializeField] private VehicleModule vehicleModule;
    
    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateUI;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateUI;
        }
    }

    private void UpdateUI(Vehicle vehicle)
    {
        vehicleModule = vehicle.GetComponent<VehicleModule>();
        UpdateReloading();
    }

    private void UpdateReloading()
    {
        reloadModule.value = vehicleModule.Rem;
    }
}
