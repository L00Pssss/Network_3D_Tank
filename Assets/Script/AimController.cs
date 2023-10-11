using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class AimController : NetworkBehaviour
{
    [SerializeField] private TankMovementDetector tankMovementDetector;

    [SerializeField] private float maxSpreadSize = 150f;
    [SerializeField] private float minSpreadSize = 90f;
    [SerializeField] private float changeDurationSeconds = 1f; // Время изменения размера в секундах
    
    
    private float progress = 0f; // Общий прогресс изменения размера от 0 до 1
    private float elapsedTime = 0f; // Время, прошедшее с начала изменения размера
    private bool isExpanding = true; // Флаг для отслеживания увеличения или уменьшения круга
    private bool isChangingSize = false; // Флаг для отслеживания увеличения или уменьшения круга
  
    public event UnityAction<float> OnUpdateNewSize;

    public float Progress => progress;
    public override void OnStartClient()
    {
        tankMovementDetector.OnUpdate += TankMovementDetectorOnOnUpdate;

        RpcUpdateNewSize(maxSpreadSize, false);

    }

    private void OnDestroy()
    {
        tankMovementDetector.OnUpdate -= TankMovementDetectorOnOnUpdate;
    }

    private void TankMovementDetectorOnOnUpdate(bool expand)
    {

        if (expand == false)
        { 
            Debug.Log("false = Ув");
            isChangingSize = expand;
            RpcUpdateNewSize(maxSpreadSize, false);
        }
        else
        {
            Debug.Log("true = Ум");
            isExpanding = expand;
            isChangingSize = true;

        }
    }

    private void Update()
    {
        if (isServer == true)
        {
            if (isChangingSize)
            {
                elapsedTime += (isExpanding ? 1 : -1) * Time.deltaTime;

                progress = Mathf.Clamp01(elapsedTime / changeDurationSeconds);

                float newSize = Mathf.Lerp(maxSpreadSize, minSpreadSize, progress);
                
                RpcUpdateNewSize(newSize, true);
                

                RpcUpdateProgress(progress);


                if (progress >= 1f)
                {
                    // Если прошло достаточно времени, сбрасываем elapsedTime и флаг isChangingSize
                    elapsedTime = 0f;
                    isChangingSize = false;
                }
            }
        }
    }
    
    [ClientRpc]
    private void RpcUpdateNewSize(float newSize, bool check)
    {
        // Вызывается на сервере для обновления ProgressAim для всех клиентов
        Debug.Log("newSize : " + newSize);
        OnUpdateNewSize?.Invoke(newSize);

        if (!check)
        {
            progress = 1;
        }
    }

    [ClientRpc]
    private void RpcUpdateProgress(float progress)
    {
        this.progress = progress;
    }

}
