using Mirror;
using UnityEngine;

public class Vehicle : Destructible
{
    [SerializeField] protected float maxLinerVelocity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource engineSound;

    [SerializeField] private float enginePitchModifier;


    [Header("Vehicle")]
    [SerializeField] private Transform zoomOpticPosition;
    public Transform ZoomOpticsPosition => zoomOpticPosition;

    public virtual float LinerVelocity => 0;

    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, LinerVelocity) == true) return 0;

            return Mathf.Clamp01(LinerVelocity / maxLinerVelocity);
        }
    }

    public Turret Turret;

    [SyncVar]
    private Vector3 netAimPoint;

    public void Fire()
    {
        Turret.Fire();
    }
    public Vector3 NetAimPoint
    {
        get => netAimPoint;

        set
        {
            netAimPoint = value;  // Client;
            CmdSetNetAimPoint(value); // Server;
        }
    }

    [Command]
    private void CmdSetNetAimPoint(Vector3 vector)
    {
        netAimPoint = vector;
    }

    protected Vector3 targetInputControl;

    public void SetTargetControl(Vector3 control)
    {
        targetInputControl = control.normalized;
    }

    protected virtual void Update()
    {
        UpdateEnigneSFX();
    }

    private void UpdateEnigneSFX()
    {
        if (engineSound != null)
        {
            engineSound.pitch = 1.0f + enginePitchModifier * NormalizedLinearVelocity;
            engineSound.volume = 0.5f + NormalizedLinearVelocity;
        }
    }

    public void SetVisibil(bool visible)
    {
        if (visible == true)
        {
            SetLayerToAll("Default");
        }
        else
        {
            SetLayerToAll("Tank_T-43");
        }
    }

    public void SetLayerToAll(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }   
}
