using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class GameEventCollector : NetworkBehaviour
{
    public UnityAction<Vehicle> PlayerVehicleSpawned;

    [Server]
    public void SvOnAddPlayer()
    {
        RpcSvOnAddPlayer();
    }

    [ClientRpc]
    public void RpcSvOnAddPlayer()
    {
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }
}
