using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private string layerNameToToggle;

    public string LayerName => layerNameToToggle;



    // �������������� ���.
    //private void Start()
    //{
    //    int layerIndexToToggle = LayerMask.NameToLayer(layerNameToToggle);

    //    if (layerIndexToToggle != -1)
    //    {
    //        // �������� ��� ��������� ���� � �������� �������� � ����� ������
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
        // �������� ������� ����� ����� �� ������
        int currentMask = targetCamera.cullingMask;

        // ���� ���������� �������� ����, ������������� ������� ���� ��� ����� ����
        // �����, ���������� ������� ���� ��� ����� ����
        if (enable)
            currentMask |= 1 << layerIndex;
        else
            currentMask &= ~(1 << layerIndex);

        // ������������� ����� ����� ����� � ������
        targetCamera.cullingMask = currentMask;
    }


}