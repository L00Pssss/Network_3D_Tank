using UnityEngine.UI;
using UnityEngine;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    [SerializeField] private Image reloadSlider;

    private Vector3 aimPosition;

    private Vector3 smoothedPosition; 

    [SerializeField] private float smoothSpeed = 5f; // Скорость сглаживания



    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateUI;
    }
    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateUI;
            
    }

    private void UpdateUI(Vehicle vehicle)
    {
            UpdateReloadSlider(vehicle.Turret);
            UpdateAimPosition(vehicle);
    }
    // хотя событие отрабатывается в update не совсем понимаю правильно и рацианально ли эта релизация 
    private void UpdateReloadSlider(Turret turret)
    {
          turret.Timer += OnReloadAim;
    }


    // есть ли смысл использовать таким образом или оставить его в update ?
    private void UpdateAimPosition(Vehicle newAimPosition)
    {

        aimPosition = VehicleInput.TraceAimPointWithoutPlayerVehicle(newAimPosition.Turret.LaunchPoint.position, newAimPosition.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if (result.z > 0)
        {
            result.z = 0;

            smoothedPosition = Vector3.Lerp(smoothedPosition, result, smoothSpeed * Time.deltaTime);

            aim.transform.position = smoothedPosition;
        }
    }

    private void OnReloadAim(float turret)
    {
        reloadSlider.fillAmount = turret;
    }

    // есть ли смысл в этом коде ?
    private void OnChangePositon()
    {

    }


    // Старый код

    //private void Update()
    //{
    //    if (Player.Local == null) return;

    //    if (Player.Local.ActiveVechicle == null) return;

    //    Vehicle v = Player.Local.ActiveVechicle;

    //    reloadSlider.fillAmount = v.Turret.FireTimerNormalize;

    //    aimPosition = VehicleInput.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

    //    Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

    //    if (result.z > 0)
    //    {
    //        result.z = 0;

    //        smoothedPosition = Vector3.Lerp(smoothedPosition, result, smoothSpeed * Time.deltaTime);

    //        aim.transform.position = smoothedPosition;
    //    }
    //}
}