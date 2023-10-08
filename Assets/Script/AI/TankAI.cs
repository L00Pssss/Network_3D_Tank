using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum AIBehaviourType
{
    Patrol,
    Support,
    InvaderBase
}
public class TankAI : NetworkBehaviour
{
    [SerializeField] private AIBehaviourType behaviourType;

    [Range(0, 1)] 
    [SerializeField] private float patrolChance;
    [Range(0, 1)] 
    [SerializeField] private float supportChance;
    [Range(0, 1)] 
    [SerializeField] private float invaderBaseChance;
    
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private AIMovement movement;
    [SerializeField] private AIShooter shooter;

    private Vehicle fireTarget;
    private Vector3 movementTarget;

    private int startCountTeamMember;
    private int countTeamMember;
    
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        vehicle.Destroyed += OnVehicleDestroyed;
        movement.enabled = false;
        shooter.enabled = false;

        CalcTeamMember();
        SetStartBehaviour();
    }

    private void Update()
    {
        if (isServer)
        {
            UpdateBehaviour();
        }
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        }
        
        if (vehicle != null)
        {
            vehicle.Destroyed -= OnVehicleDestroyed;
        }
    }
    
    private void OnVehicleDestroyed(Destructible arg0)
    {
        movement.enabled = false;
        shooter.enabled = false;
    }
    
    private void OnMatchStart()
    {
        movement.enabled = true;
        shooter.enabled = true;
    }

    private void CalcTeamMember()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].TeamId == vehicle.TeamId)
            {
                if (vehicles[i] != vehicle)
                {
                    startCountTeamMember++;
                    vehicles[i].Destroyed += OnTeamMemberDestroyed;
                }
            }
        }

        countTeamMember = startCountTeamMember;
    }

    private void SetStartBehaviour()
    {
        
        float chance = Random.Range(0.0f, patrolChance + supportChance + invaderBaseChance);

        if (chance >= 0.0f && chance <= patrolChance)
        {
            StartBehaviour(AIBehaviourType.Patrol);
            return;
        }

        if (chance >= patrolChance && chance <= patrolChance + supportChance)
        {
            StartBehaviour(AIBehaviourType.Support);
            return;
        }

        if (chance >= patrolChance + supportChance && chance <= patrolChance + supportChance + invaderBaseChance)
        {
            StartBehaviour(AIBehaviourType.InvaderBase);
            return;
        }
    }


    #region Behaviour

    private void StartBehaviour(AIBehaviourType type)
    {
        behaviourType = type;

        if (behaviourType == AIBehaviourType.InvaderBase)
        {
            movementTarget = AIPath.Instance.GetBasePoint(vehicle.TeamId);
        }

        if (behaviourType == AIBehaviourType.Patrol)
        {
            movementTarget = AIPath.Instance.GetRandomPatrolPoint();
        }
        if (behaviourType == AIBehaviourType.Support)
        {
            movementTarget = AIPath.Instance.GetRandomFirePoint(vehicle.TeamId);
        }
        
        movement.ResetPath();
    }

    private void OnReachedDestination()
    {
        if (behaviourType == AIBehaviourType.Patrol)
        {
            movementTarget = AIPath.Instance.GetRandomPatrolPoint();
        }
        
        movement.ResetPath();
    }
    
    private void OnTeamMemberDestroyed(Destructible destructible)
    {
        countTeamMember--;
        destructible.Destroyed -= OnTeamMemberDestroyed;

        if ((float)countTeamMember / (float)startCountTeamMember < 0.4f)
        {
            StartBehaviour(AIBehaviourType.Patrol);
        }

        if (countTeamMember <= 2)
        {
            StartBehaviour(AIBehaviourType.Patrol);
        }
    }

    private void UpdateBehaviour()
    {
        shooter.FindTarget();
        
        if (movement.ReachedDestination == true)
        {
            OnReachedDestination();
        }

        if (movement.HasPath == false)
        {
            movement.SetDestination(movementTarget);
        }
    }

    #endregion
    
}
