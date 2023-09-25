using System;
using System.Collections.Generic;
using UnityEngine;

public class VihicleVisibilityInCamera : MonoBehaviour
{
    private List<Vehicle> vehicles = new List<Vehicle>();
    
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        }
    }

    private void OnMatchStart()
    {
        vehicles.Clear();
        
        Vehicle[] allVehicle = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVehicle.Length; i++)
        {
            if(allVehicle[i] == Player.Local.ActiveVechicle) continue;
            
            vehicles.Add(allVehicle[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < vehicles.Count; i++)
        {
            bool isVisible = Player.Local.ActiveVechicle.vehicleViewer.IsVisible((vehicles[i].netIdentity));

            vehicles[i].SetVisible(isVisible);
        }
    }
}
