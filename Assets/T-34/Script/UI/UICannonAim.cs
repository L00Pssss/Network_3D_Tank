using UnityEngine.UI;
using UnityEngine;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    [SerializeField] private Image reloadSlider;

    private Vector3 aimPosition;

    private Vector3 smoothedPosition; 

    private Turret turret;

    private Vehicle vehicle;


    [SerializeField] private float smoothSpeed = 5f; // Скорость сглаживания


    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateUI;
    }

    private void UnsubscribeEvents()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateUI;
        }

        if (turret != null)
        {
            turret.Timer -= OnReloadAim;
        }

        if (vehicle != null)
        {
            vehicle.NetAimPointEvent -= OnChangePosition;
        }
    }

    private void UpdateUI(Vehicle vehicle)
    {
            UnsubscribeEvents();
            turret = vehicle.Turret;
            this.vehicle = vehicle;
            UpdateReloadSlider(turret);
            UpdateAimPosition(vehicle);
    }

    private void UpdateReloadSlider(Turret turret)
    {

          turret.Timer += OnReloadAim;
    }

    private void UpdateAimPosition(Vehicle vehicle)
    {
        vehicle.NetAimPointEvent += OnChangePosition;
    }

    private void OnReloadAim(float turret)
    {
        reloadSlider.fillAmount = turret;
    }

    // есть ли смысл в этом коде ?
    private void OnChangePosition()
    {
        aimPosition = VehicleInput.TraceAimPointWithoutPlayerVehicle(vehicle.Turret.LaunchPoint.position, vehicle.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if (result.z > 0)
        {
            result.z = 0;

            smoothedPosition = Vector3.Lerp(smoothedPosition, result, smoothSpeed * Time.deltaTime);

            aim.transform.position = smoothedPosition;
        }
    }
}