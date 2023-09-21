using System;
using System.Collections.Generic;
using UnityEngine;

public class VihicleVisibilityInCamera : MonoBehaviour
{
    private List<Vehicle> vehicle = new List<Vehicle>();
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
        vehicle.Clear();
        
        Vehicle[] allVehicle = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVehicle.Length; i++)
        {
            if(allVehicle[i] == Player.Local.ActiveVechicle) continue;
            
            vehicle.Add(allVehicle[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < vehicle.Count; i++)
        {
            bool isVisible = Player.Local.ActiveVechicle.vehicleViewer.IsVisible((vehicle[i].netIdentity));
            
            vehicle[i].SetVisible(isVisible);
        }
    }
}
