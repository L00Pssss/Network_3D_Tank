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
}
