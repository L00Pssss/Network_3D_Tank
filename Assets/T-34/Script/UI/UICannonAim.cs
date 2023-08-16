using UnityEngine.UI;
using UnityEngine;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    private Vector3 aimPosition;

    private void Update()
    {
        if (Player.Local == null) return;

        if(Player.Local.ActiveVechicle == null) return;

        Vehicle v = Player.Local.ActiveVechicle;

        aimPosition = VehicleInput.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if (result.z > 0)
        {
            result.z = 0;

            aim.transform.position = result;
        }
    }
}
