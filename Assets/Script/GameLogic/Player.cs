using Mirror;
using UnityEngine.Events;
using UnityEngine;
using System;
using NeworkChat;

[Serializable]
public class PlayerData
{
    public int Id;
    public string Nickname;
    public int TeamId;
    public PlayerData(int id, string nickname, int teamId)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
    }
}

public static class PlayerDataWriterRead
{
    public static void WritePlayerData(this NetworkWriter writer, PlayerData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
    }

    public static PlayerData ReadPlayerData(this NetworkReader reader)
    {
        return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
    }
}



public class Player : NetworkBehaviour
{
    private static int TeamIdCounter;

    public event UnityAction<Vehicle> VehicleSpawned;
    public event UnityAction<ProjectileHitResult> ProjectileHit; 

    public static UnityAction<int, int> ChangeFrags;
    public static Player Local
    {
        get
        {
            var x = NetworkClient.localPlayer;

            if(x != null)
                return x.GetComponent<Player>();

            return null;
        }
    }



    [SerializeField] private Vehicle[] Vehicleprefab;
    [SerializeField] private VehicleInput vehicleInput;

    public Vehicle ActiveVechicle {get;set;}

    [Header("Player")]
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string Nickname;


    [SyncVar]
    [SerializeField] private int teamId;
    public int TeamId => teamId;

    [SyncVar(hook = nameof(OnFragsChanged))]
    private int frags;
    public int Frags
    {
        set
        {
            frags = value;

            //Server
            ChangeFrags?.Invoke((int)netId, frags);
        }
        get { return frags; }
    }

    [Server]
    public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
    {
        ProjectileHit?.Invoke(hitResult);
        
        RpcInvokeProjectileHit(hitResult.Type, hitResult.Damage, hitResult.Point);
    }

    [ClientRpc]
    public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();
        hitResult.Damage = damage;
        hitResult.Type = type;
        hitResult.Point = hitPoint;
        
        ProjectileHit?.Invoke(hitResult);
    }

    //Client
    private void OnFragsChanged(int old, int newVal)
    {
        ChangeFrags?.Invoke((int)netId, newVal);
    }

    private PlayerData data;
    public PlayerData Data => data;
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

    public override void OnStopServer()
    {
        base.OnStopServer();

        PlayerList.Instance.SvRemovePlayer(data);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

            data = new PlayerData((int) netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamId);

            CmdAddPlayer(Data);

            CmdUpdateData(Data);
        }
    }

    [Command]
    private void CmdAddPlayer(PlayerData data)
    {
        PlayerList.Instance.SvAddPlayer(data);
    }

    [Command]
    private void CmdUpdateData(PlayerData data)
    {
        this.data = data;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isOwned)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private void OnMatchEnd()
    {
        if (ActiveVechicle != null)
        {
            ActiveVechicle.SetTargetControl(Vector3.zero);
            vehicleInput.enabled = false;
        }
    }

    private void Update()
    {
        if (isLocalPlayer == true)
        {
            if (ActiveVechicle != null)
            {
                ActiveVechicle.SetVisibil(!VehicleCamera.Instance.IsZoom);
            }
        }

        if (isServer == true)
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                NetworkSessionManager.Match.SvRestartMatch();
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

        GameObject vehicle = Vehicleprefab[UnityEngine.Random.Range(0, Vehicleprefab.Length)].gameObject;
        if (vehicle == null)
        {
            Debug.Log("Not Exist Prefabs");
        }
        GameObject playerVehicle = Instantiate(vehicle, transform.position, Quaternion.identity);

        playerVehicle.transform.position = teamId % 2 == 0 ? NetworkSessionManager.Instance.RandomSpawnPointGreen : NetworkSessionManager.Instance.RandomSpawnZonesBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVechicle = playerVehicle.GetComponentInParent<Vehicle>();
        ActiveVechicle.Owner = netIdentity;

        RpcSetVehicle(ActiveVechicle.netIdentity); // передача клиенту. 
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        ActiveVechicle = vehicle.GetComponent<Vehicle>();
        ActiveVechicle.Owner = netIdentity;

        if (ActiveVechicle != null && ActiveVechicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVechicle); // передаем камеру. 
        }

        vehicleInput.enabled = true;

        VehicleSpawned?.Invoke(ActiveVechicle);
    }



}
