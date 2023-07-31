using UnityEngine;

public class VehicleInput : MonoBehaviour
{
    [SerializeField] private Vehicle m_Vehicle;

    protected virtual void Update()
    {
        m_Vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
    }

}
