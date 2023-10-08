using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(Player))]
public class VehicleInput : MonoBehaviour
{
    public const float AimDistance = 1000;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("NOL");
        }
    }
    
    private int selectedProjectileIndex = -1;
    
    private Dictionary<KeyCode, int> keyToProjectileIndex = new Dictionary<KeyCode, int>
    {
        {KeyCode.Alpha1, 0},
        {KeyCode.Alpha2, 1},
        {KeyCode.Alpha3, 2}
    };
    
    

    protected virtual void Update()
    {
        if (player == null) return;

        if(player.ActiveVehicle == null) return;

        if (player.isOwned && player.isLocalPlayer)
        {
            player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
            player.ActiveVehicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);
            if (Input.GetMouseButton(0) == true)
            {
                player.ActiveVehicle.Fire();
            }

            foreach (var kvp in keyToProjectileIndex)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    if (selectedProjectileIndex == kvp.Value)
                    {
                        ConfirmSelection();
                    }
                    else
                    {
                        selectedProjectileIndex = kvp.Value;
                    }
                }
            }
            
            
        }
    }
    
    void ConfirmSelection()
    {
       // Debug.Log((selectedProjectileIndex));
        if (selectedProjectileIndex != -1)
        {
            player.ActiveVehicle.Turret.SetSelectProjectile(selectedProjectileIndex);
            selectedProjectileIndex = -1;
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

        var playerRigidbody = Player.Local.ActiveVehicle;

        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if(hits[i].collider.isTrigger == true)
                continue;
            if(hits[i].collider.transform.root.GetComponent<Vehicle>() == playerRigidbody)
                continue;

            return hits[i].point;
        }

        return ray.GetPoint((AimDistance));
    }
}
