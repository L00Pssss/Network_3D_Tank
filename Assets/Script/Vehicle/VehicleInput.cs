using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(Player))]
public class VehicleInput : MonoBehaviour
{
    public const float AimDistance = 1000;

    private Player player;

    public UnityAction<Vector3> AimPositionChanged;

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

        if(player.ActiveVechicle == null) return;

        if (player.isOwned && player.isLocalPlayer)
        {
            player.ActiveVechicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
            player.ActiveVechicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);
            if (Input.GetMouseButton(0) == true)
            {
                player.ActiveVechicle.Fire();
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
        if (selectedProjectileIndex != -1)
        {
            player.ActiveVechicle.Turret.SetSelectProjectile(selectedProjectileIndex);
            selectedProjectileIndex = -1;
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

        var playerRigidbody = Player.Local.ActiveVechicle;

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
