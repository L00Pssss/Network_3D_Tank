using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    [SerializeField] private Image reloadSlider;

    private Vector3 aimPosition;
    private Vector3 targetPosition; // Целевая позиция для сглаживания

    [SerializeField] private float smoothSpeed = 5f; // Скорость сглаживания

    private void Update()
    {
        if (Player.Local == null) return;

        if(Player.Local.ActiveVechicle == null) return;

        Vehicle v = Player.Local.ActiveVechicle;

        reloadSlider.fillAmount = v.Turret.FireTimerNormalize;

        aimPosition = VehicleInput.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if (result.z > 0)
        {
            result.z = 0;

            Vector3 smoothedPosition = Vector3.Lerp(aim.transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            aim.transform.position = result;
        }
    }
}
