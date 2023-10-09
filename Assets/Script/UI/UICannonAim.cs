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

    [SerializeField] private float maxSpreadSize = 150f;
    [SerializeField] private float minSpreadSize = 90f;
    [SerializeField] private float changeDurationSeconds = 1f; // Время изменения размера в секундах
    private float progress = 0f; // Общий прогресс изменения размера от 0 до 1
    private float elapsedTime = 0f; // Время, прошедшее с начала изменения размера
    private bool isExpanding = true; // Флаг для отслеживания увеличения или уменьшения круга
    private bool isChangingSize = false; // Флаг для отслеживания увеличения или уменьшения круга

    public float ProgressAim => progress;
    
    private void Update()
    {
        if (isChangingSize)
        {
            elapsedTime += (isExpanding ? 1 : -1) * Time.deltaTime;
            
            progress = Mathf.Clamp01(elapsedTime / changeDurationSeconds);
            
            float newSize = Mathf.Lerp(maxSpreadSize, minSpreadSize, progress);
            
            aim.rectTransform.sizeDelta = new Vector2(newSize, newSize);

            if (progress >= 1f)
            {
                // Если прошло достаточно времени, сбрасываем elapsedTime и флаг isChangingSize
                elapsedTime = 0f;
                isChangingSize = false;
            }
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            StartChangingSize(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartChangingSize(false); // Уменьшение
        }
    }

    public void StartChangingSize(bool expand)
    {
        if (expand == false)
        {
            aim.rectTransform.sizeDelta = new Vector2(maxSpreadSize, maxSpreadSize);
            isChangingSize = expand;
        }
        else
        {
            isExpanding = expand;
            isChangingSize = true;

        }

    }
    
    


    private void Start()
    {
        
        aim.rectTransform.sizeDelta = new Vector2(maxSpreadSize, maxSpreadSize);

        NetworkSessionManager.Events.PlayerVehicleSpawned += UpdateUI;
    }

    private void OnDestroy()
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

    private void OnReloadAim(float normalizedTimer)
    {
        reloadSlider.fillAmount = normalizedTimer;
    }
    
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