using UnityEngine;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : MonoBehaviour
{
    private TrackTank tank;

    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }
}
