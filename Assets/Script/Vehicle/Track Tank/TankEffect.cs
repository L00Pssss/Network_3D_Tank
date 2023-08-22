using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEffect : MonoBehaviour
{
    private TrackTank tank;

    [SerializeField] private ParticleSystem[] exhaust;
    [SerializeField] private ParticleSystem[] exhaustAtMovementStart;

    [SerializeField] private Vector2 minMaxExhaustEmission;

    private bool isTankStoped;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void Update()
    {
        float exhaustEmission = Mathf.Lerp(minMaxExhaustEmission.x, minMaxExhaustEmission.y, tank.NormalizedLinearVelocity);


        for (int i = 0; i < exhaust.Length; i++)
        {
            ParticleSystem.EmissionModule emission = exhaust[i].emission;
            emission.rateOverTime = exhaustEmission;
        }

        if (tank.LinerVelocity < 0.1f)
        {
            isTankStoped = true;
        }

        if (tank.LinerVelocity > 1f)
        {
            if (isTankStoped == true)
            {
                for (int i = 0; i < exhaustAtMovementStart.Length; i++)
                {
                    exhaustAtMovementStart[i].Play();
                }
            }

            isTankStoped = false;
        }
    }

}
