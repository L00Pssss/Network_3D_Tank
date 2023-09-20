using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    [SerializeField] private float viewDistance;

    [SerializeField] private Transform[] viewPoints;

    [SerializeField] private Color color;

    private Vehicle vehicle;

    public List<VehicleDimensions> allVehicleDimensionsList = new List<VehicleDimensions>(); // debug
    
    public SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>(); // debug

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
                allVehicleDimensionsList.Add(vehicleDimensions);
            else
                visibleVehicles.Add(vehicleDimensions.Vehicle.netIdentity);
        }
    }

    private void Update()
    {
        if(isServer == false) return;

        for (int i = 0; i < allVehicleDimensionsList.Count; i++)
        {
            bool IsVivble = false;

            for (int j = 0; j < viewPoints.Length; j++)
            {
                IsVivble = CheckVisibility(viewPoints[i].position, allVehicleDimensionsList[i]);
                
                if(IsVivble == true) break;
            }

            if (IsVivble == true && visibleVehicles.Contains(allVehicleDimensionsList[i].Vehicle.netIdentity) == false)
            {
                visibleVehicles.Add(allVehicleDimensionsList[i].Vehicle.netIdentity);
            }
            
            if (IsVivble == false && visibleVehicles.Contains(allVehicleDimensionsList[i].Vehicle.netIdentity) == true)
            {
                visibleVehicles.Remove(allVehicleDimensionsList[i].Vehicle.netIdentity);
            }
        }
    }

    public bool IsVisible(NetworkIdentity identity)
    {
        return visibleVehicles.Contains(identity);
    }

    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions)
    {
        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);

        if (distance > viewDistance) return false;

        return vehicleDimensions.IsVisibleFromPoint(transform.root, viewPoint, color);
    }
}
