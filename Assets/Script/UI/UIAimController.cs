using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIAimController : MonoBehaviour
{

    [SerializeField] private Image aim; //[SerializeField] for test
    
    private AimController aimController;
    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateVehicleModules;
        
        aim = GetComponent<Image>();
    }



    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateVehicleModules;
        }

        if (aimController != null)
        {
            aimController.OnUpdateNewSize -= UpdateAim;
        }
    }


    private void UpdateVehicleModules(Vehicle vehicle)
    {
        aimController = vehicle.GetComponent<AimController>();
        SubscribeToAimUpdates(aimController);
        
        if (vehicle.isLocalPlayer)
        {
            // Вызываем метод для настройки начального размера прицела
        }
    }

    private void SubscribeToAimUpdates(AimController newProgress)
    {
        newProgress.OnUpdateNewSize += UpdateAim;
    }
    

    private void UpdateAim(float newSize)
    {
        aim.rectTransform.sizeDelta = new Vector2(newSize, newSize);
    }
}

