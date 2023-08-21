using UnityEngine.UI;
using UnityEngine;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    [SerializeField] private Image reloadSlider;

    private Vector3 aimPosition;

    private Vector3 smoothedPosition; 

    private Turret turret;

    [SerializeField] private float smoothSpeed = 5f; // �������� �����������



    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateUI;
    }
    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= UpdateUI;
        if (turret != null)
            turret.Timer -= OnReloadAim;


    }

    private void UpdateUI(Vehicle vehicle)
    {
           turret = vehicle.Turret;
            UpdateReloadSlider(turret);
            UpdateAimPosition(vehicle);
    }
    // ���� ������� �������������� � update �� ������ ������� ��������� � ����������� �� ��� ��������� 
    private void UpdateReloadSlider(Turret turret)
    {

          turret.Timer += OnReloadAim;
    }


    // ���� �� ����� ������������ ����� ������� ��� �������� ��� � update ?
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

    // ���� �� ����� � ���� ���� ?
    private void OnChangePositon()
    {

    }


    // ������ ���

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