using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeamBase : MonoBehaviour
{
    [SerializeField] private float captureLevel;
    [SerializeField] private float captureAmountPerVehicle;

    [SerializeField] private int teamId;

    public float CaptureLevel => captureLevel;

    [SerializeField] private List<Vehicle> allvehicles = new List<Vehicle>();


    private void OnTriggerEnter(Collider other)
    {
        Vehicle vehicle = other.transform.root.GetComponent<Vehicle>();

        if (vehicle == null) return;

        if (vehicle.HitPoint == 0) return;

        if(allvehicles.Contains(vehicle)) return;

        if(vehicle.Owner.GetComponent<Player>().TeamId == teamId) return;

        vehicle.HitPointChange += OnHitPointChange;

        allvehicles.Add(vehicle);
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle vehicle = other.transform.root.GetComponent<Vehicle>();

        if(vehicle == null) return;

        vehicle.HitPointChange -= OnHitPointChange;

        allvehicles.Remove(vehicle);
    }

    private void OnHitPointChange(float hitpoint)
    {
        captureLevel = 0;

    }

    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer == true)
        {
            bool isAllDead = true;

            for (int i = 0; i < allvehicles.Count; i++)
            {
                if (allvehicles[i].HitPoint != 0)
                {
                    isAllDead = false;

                    captureLevel += captureAmountPerVehicle * Time.deltaTime;
                    captureLevel = Mathf.Clamp(captureLevel, 0, 100);


                }
            }

            if (allvehicles.Count == 0 || isAllDead == true)
            {
                captureLevel = 0;
            }
        }
    }

    public void Reset()
    {
        captureLevel = 0;

        for (int i = 0; i < allvehicles.Count; i++)
        {
            allvehicles[i].HitPointChange -= OnHitPointChange;
        }
        allvehicles.Clear();
    }
}
