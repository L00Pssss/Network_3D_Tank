using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Vehicle : Destructible
{
    public UnityAction NetAimPointEvent;

    [SerializeField] protected float maxLinerVelocity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource engineSound;

    [SerializeField] private float enginePitchModifier;


    [Header("Vehicle")]
    [SerializeField] private Transform zoomOpticPosition;
    public Transform ZoomOpticsPosition => zoomOpticPosition;

    public virtual float LinerVelocity => 0;

    protected float syncLinearVelocity;
    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, syncLinearVelocity) == true) return 0;

            return Mathf.Clamp01(syncLinearVelocity / maxLinerVelocity);
        }
    }

    public Turret Turret;

    public VehicleViewer vehicleViewer;

    public int TeamId;

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
            
            if (Owner == true)
            {
                CmdSetNetAimPoint(value); // Server;
            }

            NetAimPointEvent?.Invoke();
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

    public void SetVisible(bool visible)
    {
        if (visible == true)
        {
            if(gameObject.layer == LayerMask.NameToLayer("Default")) return;
            SetLayerToAll("Default");
        }
        else
        {
            if(gameObject.layer == LayerMask.NameToLayer("Tank_T-43")) return;
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
    
    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {

    }
}
