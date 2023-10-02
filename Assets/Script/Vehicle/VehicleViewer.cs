using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    private const float UPDATE_INTERVAL = 0.5f;
    
    private const float X_RAY_DISTANCE = 50.0f;

    private const float BASE_EXIT_TIME_FROM_DISCOVERY = 10.0f;

    private const float CAMOUFLAGE_DISTANCE = 150.0F;
    
    [SerializeField] private float viewDistance;

    [SerializeField] private Transform[] viewPoints;

    [SerializeField] private Color color;

    private Vehicle vehicle;

    private float remainingTimeLastUpdate;

    public List<VehicleDimensions> allVehicleDimensions = new List<VehicleDimensions>(); // debug
    
    public SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>(); // debug

    public List<float> remainingTime = new List<float>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        vehicle = GetComponent<Vehicle>();
        
        NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
    }



    public override void OnStopServer()
    {
        base.OnStopServer();
        
        NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
    }
    
    private void OnSvMatchStart()
    {
        color = Random.ColorHSV();

        Vehicle[] allVehicle = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVehicle.Length; i++)
        {
            if(vehicle == allVehicle[i]) continue;

            VehicleDimensions vehicleDimensions = allVehicle[i].GetComponent<VehicleDimensions>();
            
            if(vehicleDimensions == null) continue;

            if (vehicle.TeamId != allVehicle[i].TeamId)
                allVehicleDimensions.Add(vehicleDimensions);
            else
            {
                visibleVehicles.Add(vehicleDimensions.Vehicle.netIdentity);
                remainingTime.Add(-1);
            }
        }
    }

    private void Update()
    {
        if(isServer == false) return;

        remainingTimeLastUpdate += Time.deltaTime;

        if (remainingTimeLastUpdate >= UPDATE_INTERVAL)
        {
            for (int i = 0; i < allVehicleDimensions.Count; i++)
            {
                if (allVehicleDimensions[i].Vehicle == null) continue;

                bool IsVisible = false;

                for (int j = 0; j < viewPoints.Length; j++)
                {
                    IsVisible = CheckVisibility(viewPoints[j].position, allVehicleDimensions[i]);

                    if (IsVisible == true) break;
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == false)
                {
                    visibleVehicles.Add(allVehicleDimensions[i].Vehicle.netIdentity);
                    remainingTime.Add(-1);
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = -1;
                }

                if (IsVisible == false && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    if (remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] == -1)
                    {
                        remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] =
                            BASE_EXIT_TIME_FROM_DISCOVERY;
                    }
                }
            }

            for (int i = 0; i < remainingTime.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime;
                    if (remainingTime[i] <= 0)
                        remainingTime[i] = 0;
                }

                if (remainingTime[i] == 0)
                {
                    remainingTime.RemoveAt(i);
                    visibleVehicles.RemoveAt(i);
                }
            }

            remainingTimeLastUpdate = 0;
        }
    }
    
    

    public bool IsVisible(NetworkIdentity identity)
    {
        return visibleVehicles.Contains(identity);
    }
    
    public List<Vehicle> GetAllVehicle()
    {
        List<Vehicle> allVehicle = new List<Vehicle>(allVehicleDimensions.Count);
        for(int i = 0; i <  allVehicleDimensions.Count; i++)
        {
            allVehicle.Add(allVehicleDimensions[i].Vehicle);
        }

        return allVehicle;
    }
    
    public List<Vehicle> GetAllVisibleVehicle()
    {
        List<Vehicle> allVisibleVehicle = new List<Vehicle>(allVehicleDimensions.Count);
        for(int i = 0; i <  visibleVehicles.Count; i++)
        {
            allVisibleVehicle.Add(visibleVehicles[i].GetComponent<Vehicle>());
        }

        return allVisibleVehicle;
    }
    
    

    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions)
    {
        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);

        if (Vector3.Distance(viewPoint, vehicleDimensions.transform.position) <= X_RAY_DISTANCE) return true;
            
        if (distance > viewDistance) return false;

        float currentViewDistance = viewDistance;

        if (distance >= CAMOUFLAGE_DISTANCE)
        {
            VehicleComuflage vehicleComuflage = vehicleDimensions.Vehicle.GetComponent<VehicleComuflage>();
            
            if(vehicleComuflage != null)
                currentViewDistance = viewDistance - vehicleComuflage.CurrentDistance;
        }

        if (distance > currentViewDistance) return false;

        return vehicleDimensions.IsVisibleFromPoint(transform.root, viewPoint, color);
    }

}
