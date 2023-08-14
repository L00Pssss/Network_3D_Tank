using UnityEngine;

[RequireComponent (typeof(Player))]
public class VehicleInput : MonoBehaviour
{
    private Player player;

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
        }
    }

}
