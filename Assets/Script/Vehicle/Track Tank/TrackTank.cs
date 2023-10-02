using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class TrackWhellRow
{
    [SerializeField] private WheelCollider[] colliders;

    [SerializeField] private Transform[] meshs;

    public float minRpm;
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
        //Find min rpm

        List<float> allRpm = new List<float>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isGrounded == true)
            {
                allRpm.Add(colliders[i].rpm);
            }
        }

        if (allRpm.Count > 0)
        {
            minRpm = Mathf.Abs(allRpm[0]);

            for (int i = 0; i < allRpm.Count; i++)
            {
                if (Mathf.Abs(allRpm[i]) < minRpm)
                {
                    minRpm = Mathf.Abs(allRpm[i]);
                }
            }

            minRpm = minRpm * Mathf.Sign(allRpm[0]);
        }

        float angel = minRpm * 360.0f / 60.0f * Time.fixedDeltaTime;

        for (int i = 0; i < meshs.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);

            meshs[i].position = position;
            meshs[i].Rotate(angel, 0, 0);
        }
    }

    public void UpdateMeshRotationByRpm(float rpm)
    {
        float angel = rpm * 360.0f / 60.0f * Time.fixedDeltaTime;

        for (int i = 0; i < meshs.Length; i++)
        {
            Vector3 positon;
            Quaternion rotation;

            colliders[i].GetWorldPose(out positon, out rotation);

            meshs[i].position = positon;
            meshs[i].Rotate(angel, 0, 0);
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class TrackTank : Vehicle
{
    public override float LinerVelocity => rigidBody.velocity.magnitude;

    [SerializeField] private Transform centerOfMass;

    [Header("Tracks")]
    [SerializeField] private TrackWhellRow leftWheelRow;
    [SerializeField] private TrackWhellRow rightWheelRow;
    [SerializeField] protected GameObject destoredPrefab;
    [SerializeField] private GameObject visualModel;

    [Header("Movement")]
    [SerializeField] private ParameterCurve forwardTorqueCurve;
    [SerializeField] private float maxForwardTorque;
    [SerializeField] private float maxBackwardMotorTorque;
    [SerializeField] private ParameterCurve backwardTorqueCurve;
    [SerializeField] private float breakTorque;
    [SerializeField] private float rollingResistance;


    [Header("Rotation")]
    [SerializeField] private float rotateTorqueInPlase;
    [SerializeField] private float rotateBreakInPlase;
    [Space(10)]
    [SerializeField] private float rotateTorqueInMotion;
    [SerializeField] private float rotateBreakInMotion;

    [Header("Friction")]
    [SerializeField] private float minSidewayStiffnessInPlace;
    [SerializeField] private float minSidewayStiffnessInMotion;
    [SerializeField] private float currentMotorTorque;
    
    
    private Rigidbody rigidBody;
    
    private bool isMobile = true; // ѕо умолчанию машина активна

    public bool IsMobile
    {
        get { return isMobile; }
        set { isMobile = value; }
    }
    
    
    


    public float LeftWheelRmp => leftWheelRow.minRpm;
   
    public float RighttWheelRmp => rightWheelRow.minRpm;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
       
        rigidBody.centerOfMass = centerOfMass.localPosition;
        Destroyed += OnTrackTankDestroyed;
    }

    private void OnDestroy()
    {
        Destroyed -= OnTrackTankDestroyed;
    }

    private void OnTrackTankDestroyed(Destructible destructible)
    {
        GameObject ruinedVisualModel = Instantiate(destoredPrefab);
        ruinedVisualModel.transform.position = visualModel.transform.position;
        ruinedVisualModel.transform.rotation = visualModel.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (isServer == true)
        {
            UpdateMotorTorque();

            SvUpdateWheelRpm(LeftWheelRmp, RighttWheelRmp);

            SvUpdateLinearVelocity(LinerVelocity);
        }
        
        if (isOwned == true)
        {
            UpdateMotorTorque();

            CmdUpdateWheelRpm(LeftWheelRmp, RighttWheelRmp);

            CmdUpdateLinearVelocity(LinerVelocity);
        }
    }
    
    [Command]
    private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        SvUpdateWheelRpm(leftRpm, rightRpm);
    }

    [Command]
    private void CmdUpdateLinearVelocity(float velocity)
    {
        SvUpdateLinearVelocity(velocity);
    }
    
    [Server]
    private void SvUpdateLinearVelocity(float velocity)
    {
        syncLinearVelocity = velocity;
    }

    [Server]
    private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        RpcUpdateWheelRpm(leftRpm, rightRpm);
    }
    


    [ClientRpc(includeOwner = false)]
    private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        leftWheelRow.minRpm = leftRpm;
        rightWheelRow.minRpm = rightRpm;

        leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
        rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
    }

    private void UpdateMotorTorque()
    {
        float targetMotorTorque = targetInputControl.z > 0 ? maxForwardTorque * Mathf.RoundToInt(targetInputControl.z) : maxBackwardMotorTorque * Mathf.RoundToInt(targetInputControl.z); // тернарный оператор
        float breakTorque = this.breakTorque * targetInputControl.y;
        float steering = targetInputControl.x;


        if (!isMobile)
        {
            targetMotorTorque = 0;
            steering = 0;
        }

        // Update target motor torque
        if (targetMotorTorque > 0)
        {
            currentMotorTorque = forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }
        if (targetMotorTorque < 0)
        {
            currentMotorTorque = backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque == 0)
        {
            currentMotorTorque = backwardTorqueCurve.Reset();
            currentMotorTorque = forwardTorqueCurve.Reset();
        }



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
            if (steering != 0 && (Mathf.Abs(leftWheelRow.minRpm) < 1 || Mathf.Abs(rightWheelRow.minRpm) < 1))
            {
                rightWheelRow.SetTorque(rotateTorqueInPlase);
                leftWheelRow.SetTorque(rotateTorqueInPlase);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInPlase);
                    rightWheelRow.SetTorque(rotateTorqueInPlase);
                }
                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInPlase);
                    rightWheelRow.Break(rotateBreakInPlase);
                }
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }


        // Move
        // Move
        if (targetMotorTorque != 0)
        {
            if (steering == 0)
            {
                if (LinerVelocity < maxLinerVelocity)
                {
                    leftWheelRow.SetTorque(currentMotorTorque);
                    rightWheelRow.SetTorque(currentMotorTorque);
                }
            }

            if (steering != 0 && (Mathf.Abs(leftWheelRow.minRpm) < 1 || Mathf.Abs(rightWheelRow.minRpm) < 1))
            {
                rightWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                leftWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInMotion);
                    rightWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                }

                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                    rightWheelRow.Break(rotateBreakInMotion);
                }
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }

        leftWheelRow.UpdateMeshTransform();
        rightWheelRow.UpdateMeshTransform();
    }
}
