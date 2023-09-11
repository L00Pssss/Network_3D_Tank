using System;
using UnityEngine;

public class VehicleModule : Destructible
{
    [SerializeField] private string title;
    [SerializeField] private Armor armor;
    [SerializeField] private float recoveredTime;

    private float remainingRecoveryTime;
    public float Rem => remainingRecoveryTime / recoveredTime;

    private void Awake()
    {
        armor.SetDestuctible(this);
    }

    private void Start()
    {
        Destroyed += OnModuleDestroyed;
        enabled = false;
    }

    private void OnDestroy()
    {
        Destroyed -= OnModuleDestroyed;
    }

    private void Update()
    {
        if (isServer == true)
        {
            remainingRecoveryTime -= Time.deltaTime;

            if (remainingRecoveryTime <= 0)
            {
                remainingRecoveryTime = 0.0f;
                
                SvRecovery();

                enabled = false;
            }
        }
        
        Debug.Log(Rem);
    }

    private void OnModuleDestroyed(Destructible arg0)
    {
        remainingRecoveryTime = recoveredTime;
        enabled = true;
    }
}
