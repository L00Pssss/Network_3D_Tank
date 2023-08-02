using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private string layerNameToToggle;

    public string LayerName => layerNameToToggle;



    // Необязательный код.
    //private void Start()
    //{
    //    int layerIndexToToggle = LayerMask.NameToLayer(layerNameToToggle);

    //    if (layerIndexToToggle != -1)
    //    {
    //        // Включить или выключить слой с заданным индексом в маске камеры
    //        EnableLayer(layerIndexToToggle, true);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Layer name not found!");
    //    }
    //}


    public void SetLayerState(bool enable)
    {
        int layerIndexToToggle = LayerMask.NameToLayer(layerNameToToggle);

        if (layerIndexToToggle != -1)
        {
            EnableLayer(layerIndexToToggle, enable);
        }
        else
        {
            Debug.LogWarning("Layer name not found!");
        }
    }

    private void EnableLayer(int layerIndex, bool enable)
    {
        // Получаем текущую маску слоев из камеры
        int currentMask = targetCamera.cullingMask;

        // Если необходимо включить слой, устанавливаем битовый флаг для этого слоя
        // Иначе, сбрасываем битовый флаг для этого слоя
        if (enable)
            currentMask |= 1 << layerIndex;
        else
            currentMask &= ~(1 << layerIndex);

        // Устанавливаем новую маску слоев в камеру
        targetCamera.cullingMask = currentMask;
    }


}