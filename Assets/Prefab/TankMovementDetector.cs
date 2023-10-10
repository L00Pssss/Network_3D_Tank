using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TankMovementDetector : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin; // Позиция, откуда будет выпущен луч
    [SerializeField] private float raycastInterval = 0.5f; // Интервал времени для проверки движения
    [SerializeField] private float maxStillTime = 2.0f; // Максимальное время бездействия для увеличения прицела
    [SerializeField] private float positionTolerance = 0.2f; // Погрешность в положении
    
    public event UnityAction<bool> OnUpdate;

    [FormerlySerializedAs("cannonAim")] [SerializeField] private UIAimController cannonUIAim;

    private float timeSinceLastHit; // Время с момента последнего попадания луча
    private Vector3 lastHitPosition; // Предыдущая позиция точки попадания луча

    private string nameMethod = "CheckTankMovement";

    private bool checking = true;

    private void Start()
    {
        // Инициализируем начальные значения
        timeSinceLastHit = 0f;
        lastHitPosition = raycastOrigin.position;

        // Запускаем проверку движения через определенные интервалы времени
        InvokeRepeating(nameMethod, raycastInterval, raycastInterval);
    }

    private void CheckTankMovement()
    {
        // Выпускаем луч из позиции raycastOrigin в направлении движения танка
        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Проверяем, изменилась ли позиция точки попадания луча с учетом погрешности
            if (Vector3.Distance(hit.point, lastHitPosition) > positionTolerance)
            {
              
                // Танк движется, сбрасываем время бездействия
                if (checking == true)
                {
                    OnUpdate?.Invoke(false);
                    checking = false;
                }

                timeSinceLastHit = 0f;
            }
            else
            {
                // Танк стоит на месте, увеличиваем время бездействия
                timeSinceLastHit += raycastInterval;

                // Если время бездействия достигло максимального значения, увеличиваем прицел
                if (timeSinceLastHit >= maxStillTime)
                {
                    // Здесь можно выполнить логику увеличения прицела
                    if (checking == false)
                    {
                        OnUpdate?.Invoke(true);
                        checking = true;
                    }
                    
                    Debug.Log("Tank is still for too long, increase the crosshair!");
                }
            }

            // Обновляем позицию точки попадания луча
            lastHitPosition = hit.point;
        }
    }
}
