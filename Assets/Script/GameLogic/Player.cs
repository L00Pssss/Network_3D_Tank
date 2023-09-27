using Mirror;
using UnityEngine.Events;
using UnityEngine;

public class Player : MatchMember
{
    public event UnityAction<Vehicle> VehicleSpawned;
    public event UnityAction<ProjectileHitResult> ProjectileHit;

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

    public override void OnStartServer()
    {
        base.OnStartServer();

        teamId = MatchController.GetNextTeamID();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemoveMember(data);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

            data = new MatchMemberData((int) netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamId, netIdentity);

            CmdAddPlayer(Data);

            CmdUpdateData(Data);
        }
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isOwned)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    [Command]
    private void CmdAddPlayer(MatchMemberData memberData)
    {
        MatchMemberList.Instance.SvAddMember(memberData);
    }
    


    private void OnMatchEnd()
    {
        if (ActiveVehicle != null)
        {
            ActiveVehicle.SetTargetControl(Vector3.zero);
            vehicleInput.enabled = false;
        }
    }

    private void Update()
    {
        if (isLocalPlayer == true)
        {
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
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
        if (ActiveVehicle != null) return;

        GameObject vehicle = Vehicleprefab[UnityEngine.Random.Range(0, Vehicleprefab.Length)].gameObject;
        if (vehicle == null)
        {
            Debug.Log("Not Exist Prefabs");
        }
        GameObject playerVehicle = Instantiate(vehicle, transform.position, Quaternion.identity);

        playerVehicle.transform.position = teamId % 2 == 0 ? NetworkSessionManager.Instance.RandomSpawnPointGreen : NetworkSessionManager.Instance.RandomSpawnZonesBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVehicle = playerVehicle.GetComponentInParent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = teamId;

        RpcSetVehicle(ActiveVehicle.netIdentity); // передача клиенту. 
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        ActiveVehicle = vehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = teamId;

        if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle); // передаем камеру. 
        }

        vehicleInput.enabled = true;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }



}
