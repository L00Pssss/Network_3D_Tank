using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Vehicle))]
public class AIShooter : MonoBehaviour
{
    [SerializeField] private VehicleViewer vehicleViewer;
    [SerializeField] private Transform firePosition;
    [SerializeField] private int maximumDistanceFire = 650;
    
    private Vehicle vehicle;
    private Vehicle target;
    private Transform lookTransform;

    public bool HasTarget => target != null;
    
    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        FindTarget();
        LookOnTarget();
        TryFire();
    }

    private void FindTarget()
    {
        List<Vehicle> visibleVehicles  = vehicleViewer.GetAllVisibleVehicle();

        float minDistance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < visibleVehicles .Count; i++)
        {
            if (visibleVehicles[i].HitPoint == 0 || visibleVehicles[i].TeamId == this.vehicle.TeamId) continue;

            float distance = Vector3.Distance(transform.position, visibleVehicles [i].transform.position);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }

        if (index != -1)
        {
            // Проверяем, изменилась ли цель перед её установкой.
            if (target != visibleVehicles[index])
            {
                target = visibleVehicles[index];

                VehicleDimensions vehicleDimensions = target.GetComponent<VehicleDimensions>();

                if (vehicleDimensions == null) return;

                lookTransform = vehicleDimensions.GetPriorityFirePoint();
            }
        }
        else
        {
            target = null;
            lookTransform = null;
        }
    }

    private void LookOnTarget()
    {
        if (lookTransform == null) return;

        vehicle.NetAimPoint = lookTransform.position;
    }

    private void TryFire()
    {
        if(target == null) return;
        
        RaycastHit hit;

        if (Physics.Raycast(firePosition.position, firePosition.forward, out hit, maximumDistanceFire))
        {
            if (hit.collider.transform.root == target.transform.root)
            {
                vehicle.Turret.SvFire();
            }
        }
        
    }
}
