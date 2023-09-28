using System;
using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 GetPositionZX(this Transform t)
    {
        var x = t.position;
        x.y = 0;
        return x;
    }
}

public static class VectorExtensions
{
    public static Vector3 GetPositionZX(this Vector3 t)
    {
        var x = t;
        x.y = 0;
        return x;
    }
}
[RequireComponent(typeof(Vehicle))]
public class AIMovement : MonoBehaviour
{
    [SerializeField] private AIRaySensor sensorForward;
    [SerializeField] private AIRaySensor sensorBackward;
    [SerializeField] private AIRaySensor sensorRight;
    [SerializeField] private AIRaySensor sensorLeft;

    private Vector3 target;
    



    private Vehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Finish").transform.position;
        MoveToTarget();
    }
    
    private Vector3 GetReferenceMovementDirectionZX()
    {
        var tankPos = vehicle.transform.GetPositionZX();
        var targetPos = target.GetPositionZX();

        return (targetPos - tankPos).normalized;
    }

    private Vector3 GetTankDirectionZX()
    {
        var tankDir = vehicle.transform.forward.GetPositionZX();
        tankDir.Normalize();
        return tankDir;
    }


    
    private void MoveToTarget()
    {
        float turnControl = 0;
        float forwardThrust = 1;

        var referenceDirection = GetReferenceMovementDirectionZX();
        var tankDir = GetTankDirectionZX();


        var forwardSensorState = sensorForward.Raycast();
        var leftSensorState = sensorForward.Raycast();
        var rightSensorState = sensorForward.Raycast();

        if (forwardSensorState.Item1)
        {
            forwardThrust = 0;

            if (leftSensorState.Item1 == false)
            {
                turnControl = -1;
                forwardThrust = -0.2f;
            }
            else if (rightSensorState.Item1 == false)
            {
                turnControl = 1;
                forwardThrust = -0.2f;
            }
            else
            {
                forwardThrust = -1;
            }
        }

        else
        {
            turnControl = Mathf.Clamp(Vector3.SignedAngle(tankDir, referenceDirection,Vector3.up), -45.0f, 45.0f) / 45.0f;

            const float minSideDistance = 1;

            if (leftSensorState is { Item1: true, Item2: < minSideDistance } && turnControl < 0)
                turnControl = -turnControl;
            
            if (rightSensorState.Item1 && rightSensorState.Item2 < minSideDistance && turnControl > 0)
                turnControl = -turnControl;
        }

        
        vehicle.SetTargetControl(new Vector3(turnControl,0,forwardThrust));
    }
}
