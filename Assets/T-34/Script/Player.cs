using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private static int TeamIdCounter;
    public static Player Local
    {
        get
        {
            var x = NetworkClient.localPlayer;

            if(x != null)
                return x.GetComponent<Player>();

            Debug.Log("Not Exist Prefabs");
            return null;
        }
    }



    [SerializeField] 
    private Vehicle[] Vehicleprefab;

    public Vehicle ActiveVechicle {get;set;}

    [Header("Player")]
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string Nickname;


    [SyncVar]
    [SerializeField] private int teamId;
    public int TeamId => teamId;
    private void OnNicknameChanged(string old, string newVal)
    {
        gameObject.name = "Player_" + newVal; // on Client
    }

    [Command] // on Server
    public void CmdSetName(string name)
    {
        Nickname = name;
        gameObject.name = "Player_" + name;
    }

    [Command]
    public void CmdSetTeamId(int teamId)
    {
        this.teamId = teamId;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        teamId = TeamIdCounter % 2;
        TeamIdCounter++;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
        }
    }

    private void Update()
    {
        if (isServer == true)
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                foreach (var v in FindObjectsOfType<Player>())
                {
                    if (v.ActiveVechicle != null)
                    {
                        NetworkServer.UnSpawn(v.ActiveVechicle.gameObject);
                        Destroy(v.ActiveVechicle.gameObject);


                        v.ActiveVechicle = null;
                    }
                }

                foreach (var v in FindObjectsOfType<Player>())
                {
                    v.SvSpwanClintVehicle();
                }
            }
        }

        if (isOwned == true)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if(Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    [Server]
    public void SvSpwanClintVehicle()
    {
        if (ActiveVechicle != null) return;

        GameObject vehicle = Vehicleprefab[Random.Range(0, Vehicleprefab.Length)].gameObject;
        if (vehicle == null)
        {
            Debug.Log("Not Exist Prefabs");
        }
        GameObject playerVehicle = Instantiate(vehicle, transform.position, Quaternion.identity);

        playerVehicle.transform.position = teamId % 2 == 0 ? NetworkSessionManager.Instance.RandomSpawnPointRed : NetworkSessionManager.Instance.RandomSpawnZonesBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVechicle = playerVehicle.GetComponentInParent<Vehicle>();
        ActiveVechicle.Owner = netIdentity;

        RpcSetVehicle(ActiveVechicle.netIdentity); // �������� �������. 
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        ActiveVechicle = vehicle.GetComponent<Vehicle>();

        if (ActiveVechicle != null && ActiveVechicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVechicle); // �������� ������. 
        }
    }



}
