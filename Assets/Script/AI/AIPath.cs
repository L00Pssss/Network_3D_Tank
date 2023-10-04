using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPath : MonoBehaviour
{
    public static AIPath Instance;

    [SerializeField] private Transform baseRedPoint;
    [SerializeField] private Transform baseBluePoint;

    [SerializeField] private Transform[] fireRedPoint;
    [SerializeField] private Transform[] fireBluePoint;

    [SerializeField] private Transform[] patrolPoint;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetBasePoint(int teamId)
    {
        return (teamId == TeamSide.teamRed) ? baseRedPoint.position : baseBluePoint.position;
    }

    public Vector3 GetRandomFirePoint(int teamId)
    {
        return (teamId == TeamSide.teamRed) ? fireRedPoint[Random.Range(0, fireRedPoint.Length)].position 
            : fireBluePoint[Random.Range(0, fireBluePoint.Length)].position;
    }

    public Vector3 GetRandomPatrolPoint()
    {
        return patrolPoint[Random.Range(0, patrolPoint.Length)].position;
    }
}
