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

            colliders[i].brakeTorque = breakTorque;
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


    public void SetSidewayStiffness(float stiffness)
    {
        WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        for (int i = 0; i < colliders.Length; i++)
        {
            wheelFrictionCurve = colliders[i].sidewaysFriction;
            wheelFrictionCurve.stiffness = stiffness;

            colliders[i].sidewaysFriction = wheelFrictionCurve;
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
[RequireComponent(typeof(Rigidbody))]
public class TrackTank : Vehicle
{
    public override float LinerVelocity => rigidBody.velocity.magnitude;

    [SerializeField] private TrackWhellRow leftWheelRow;
    [SerializeField] private TrackWhellRow rightWheelRow;

    [Header("Movement")]
    [SerializeField] private float maxForwardTorque;
    [SerializeField] private float maxBackwardMotorTorque;
    [SerializeField] private float breakTorque;
    [SerializeField] private float rollingResistance;


    [Header("Rotation")]
    [SerializeField] private float rotateTorqueInPlase;
    [SerializeField] private float rotateBreakInPlase;
    [Space(2)]
    [SerializeField] private float rotateTorqueInMotion;
    [SerializeField] private float rotateBreakInMotion;

    [Header("Friction")]
    [SerializeField] private float minSidewayStiffnessInPlace;
    [SerializeField] private float minSidewayStiffnessInMotion;

    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float targetMotorTorque = targetInputControl.z > 0 ? maxForwardTorque * Mathf.RoundToInt(targetInputControl.z) : maxBackwardMotorTorque * Mathf.RoundToInt(targetInputControl.z); // тернарный оператор
        float breakTorque  = this.breakTorque * targetInputControl.y;
        float steering    = targetInputControl.x;

        // Break
        leftWheelRow.Break(breakTorque);
        rightWheelRow.Break(breakTorque);
        // Rolling
        if (targetMotorTorque == 0 && steering == 0)
        {
            leftWheelRow.Break(rollingResistance);
            rightWheelRow.Break(rollingResistance);
        }
        else
        {
            leftWheelRow.Reset();
            rightWheelRow.Reset();
        }
        // Rotate in palce
        if (targetMotorTorque == 0 && steering != 0)
        {
            if (steering < 0)
            {
                leftWheelRow.Break(rotateBreakInPlase);
                rightWheelRow.SetTorque(rotateTorqueInPlase);
            }  
            if (steering > 0)
            {
                leftWheelRow.SetTorque(rotateTorqueInPlase);
                rightWheelRow.SetTorque(rotateBreakInPlase);
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }


        // Move
        if (targetMotorTorque != 0)
        {
            if (steering == 0)
            {
                if (LinerVelocity < maxLinerVelocity)
                {
                    leftWheelRow.SetTorque(targetMotorTorque);
                    rightWheelRow.SetTorque(targetMotorTorque);
                }
            }
        }

        leftWheelRow.UpdateMeshTransform();
        rightWheelRow.UpdateMeshTransform();
    }


}
