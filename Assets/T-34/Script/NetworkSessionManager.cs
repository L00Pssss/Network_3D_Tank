using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] spawnZonesGreen;
    [SerializeField] private SphereArea[] spawnZonesBlue;


    public Vector3 RandomSpawnPointGreen => spawnZonesGreen[Random.Range(0, spawnZonesGreen.Length)].RandomInside;
    public Vector3 RandomSpawnZonesBlue => spawnZonesBlue[Random.Range(0, spawnZonesBlue.Length)].RandomInside;

    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance.gameEventCollector;

    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);


    [SerializeField] private GameEventCollector gameEventCollector;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        gameEventCollector.SvOnAddPlayer();
    }
}
