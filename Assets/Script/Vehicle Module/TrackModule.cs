using UnityEngine;
using Mirror;

[RequireComponent(typeof(TrackTank))]
public class TrackModule : NetworkBehaviour
{
    [Header("Visual")] 
    [SerializeField] private GameObject leftTrackMesh;
    [SerializeField] private GameObject leftTrackDamageMesh;
    [SerializeField] private GameObject rightTrackMesh;
    [SerializeField] private GameObject rightTrackDamageMesh;

    [Space(5)]
    [SerializeField] private VehicleModule leftTrack;
    [SerializeField] private VehicleModule rightTrack;
    

    private TrackTank tank;

    public override void OnStartClient()
    {
     //   base.OnStartClient();
        
        tank = GetComponent<TrackTank>();
        
        leftTrack.Destroyed += OnLeftTrackDestroyed;
        rightTrack.Destroyed += OnRightTrackDestroyed;
        
        leftTrack.Recovered += OnLeftTrackRecovered;
        rightTrack.Recovered += OnRightTrackRecovered;
    }
    

    private void OnDestroy()
    {
        leftTrack.Destroyed -= OnLeftTrackDestroyed;
        rightTrack.Destroyed -= OnRightTrackDestroyed;
        
        leftTrack.Recovered -= OnLeftTrackRecovered;
        rightTrack.Recovered -= OnRightTrackRecovered;
    }

    private void OnRightTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        
        if (leftTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void OnRightTrackDestroyed(Destructible arg0)
    {
        Debug.Log("POPAL");
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        

        TakeAwayMobility();

    }

    private void OnLeftTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);
        
        if (rightTrack.HitPoint > 0)
        {
            RegainMobility();
        }
        
    }

    private void OnLeftTrackDestroyed(Destructible arg0)
    {
        Debug.Log("POPAL");
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);

        TakeAwayMobility();
        
        
        
    }

    private void ChangeActiveObjects(GameObject a, GameObject b)
    {
        a.SetActive(b.activeSelf);
        b.SetActive(!b.activeSelf);
    }

    private void TakeAwayMobility()
    {
        tank.IsMobile = false;
    //    tank.enabled = false;
    }

    private void RegainMobility()
    {
        tank.IsMobile = true;
   //     tank.enabled = true;
    }
}
