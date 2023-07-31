using UnityEngine;

[System.Serializable]
public class TrackWhellRow
{
    [SerializeField] private WheelCollider[] colliders;

    [SerializeField] private Transform[] meshs;

    public void SetTorque(float motorTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {

            colliders[i].motorTorque = motorTorque;
        }
    }

    public void Break(float breakTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {

            colliders[i].motorTorque = breakTorque;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = 0;
            colliders[i].motorTorque = 0;
        }
    }

    public void UpdateMeshTransform()
    {
        for (int i = 0; i < meshs.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);

            meshs[i].position = position;
            meshs[i].rotation = rotation;
        }
    }
}
public class TrackTank : Vehicle
{
    [SerializeField] private TrackWhellRow[] leftWheelRow;
    [SerializeField] private TrackWhellRow[] rightWheelRow;
}
