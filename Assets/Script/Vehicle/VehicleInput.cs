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
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] htis = Physics.RaycastAll(ray, AimDistance);

        var m = Player.Local.ActiveVechicle.GetComponent<Rigidbody>();

        foreach (RaycastHit hit in htis)
        {
            if (hit.rigidbody == m)
                continue;

            return hit.point;
        }

        return ray.GetPoint(AimDistance);
    }
}
