using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] spawnZonesGreen;
    [SerializeField] private SphereArea[] spawnZonesBlue;


    public Vector3 RandomSpawnPointGreen => spawnZonesGreen[Random.Range(0, spawnZonesGreen.Length)].RandomInside;
    public Vector3 RandomSpawnZonesBlue => spawnZonesBlue[Random.Range(0, spawnZonesBlue.Length)].RandomInside;
    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);
}
