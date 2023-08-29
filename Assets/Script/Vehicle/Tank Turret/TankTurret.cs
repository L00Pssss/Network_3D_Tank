using UnityEngine;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : Turret
{
    private TrackTank tank;

    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;

    [SerializeField] protected float horizontalRotationSpeed;
    [SerializeField] protected float verticalRotationSpeed;


    [SerializeField] protected float maxTopAngel;
    [SerializeField] protected float maxBottomAngel;

    [Header("SFX")]
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private ParticleSystem mazzel;
    [SerializeField] private float foreceRecoil;


    private float maskCurrentAngel;

    private Rigidbody tankRigidbody;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
        tankRigidbody = tank.GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();

        ControlTurretAim();
    }

    protected override void OnFire()
    {
        base.OnFire();

        Projectile projectile = Instantiate(Projectile);

        projectile.transform.position = launchPoint.position;
        projectile.transform.forward = launchPoint.forward;
        projectile.Owner = tank.Owner;


        FireSfx();
    }

    private void FireSfx()
    {
        fireSound.Play();
        mazzel.Play();

        tankRigidbody.AddForceAtPosition(-mask.forward * foreceRecoil, mask.position, ForceMode.Impulse);
    }

    private void ControlTurretAim()
    {
        //Tower
        Vector3 LocalPosition = tower.InverseTransformPoint(tank.NetAimPoint);
        LocalPosition.y = 0;
        Vector3 GlobalPosition = tower.TransformPoint(LocalPosition);
        tower.rotation = Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((GlobalPosition - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);

        //Mask
        mask.localRotation = Quaternion.identity;

        LocalPosition = mask.InverseTransformPoint(tank.NetAimPoint);
        LocalPosition.x = 0;
        GlobalPosition = tower.TransformPoint(LocalPosition);


        float targetAngel = -Vector3.SignedAngle((GlobalPosition - mask.position).normalized, mask.forward, mask.right);
        targetAngel = Mathf.Clamp(targetAngel, maxTopAngel, maxBottomAngel);

        maskCurrentAngel = Mathf.MoveTowards(maskCurrentAngel, targetAngel, Time.deltaTime * verticalRotationSpeed);
        mask.localRotation = Quaternion.Euler(maskCurrentAngel, 0, 0);
    }
}
