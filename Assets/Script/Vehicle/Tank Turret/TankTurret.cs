using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : Turret
{
    private TrackTank tank;

    [SerializeField] private AimController aimController;


    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;

    [SerializeField] protected float horizontalRotationSpeed;
    [SerializeField] protected float verticalRotationSpeed;


    [SerializeField] protected float maxTopAngel;
    [SerializeField] protected float maxBottomAngel;

    [Header("SFX")] [SerializeField] private AudioSource fireSound;
    [SerializeField] private ParticleSystem mazzel;
    [SerializeField] private float foreceRecoil;

    private float maskCurrentAngel;


    private Rigidbody tankRigidbody;


    private void Start()
    {
        tank = GetComponent<TrackTank>();
        tankRigidbody = tank.GetComponent<Rigidbody>();
        aimController = GetComponent<AimController>();

    }

    protected override void Update()
    {
        base.Update();

        ControlTurretAim();
    }

    protected override void OnFire()
    {
        base.OnFire();

        var projectile = Instantiate(SelectedProjectile.ProjectilePrefab); // var - тот же.

    //    Debug.Log($"projectile + {projectile.name}");

        projectile.Owner = tank.Owner;
        projectile.SetProperties(SelectedProjectile);


        projectile.transform.position = launchPoint.position;



        Vector3 finalDirection = launchPoint.forward + RandomVectorPosition();

        projectile.transform.forward = finalDirection.normalized;

        FireSfx();
    }

    private Vector3 RandomVectorPosition()
    {
        //  минимальный и максимальный разброс в градусах
        float minDeviationDegrees = 0f; // Минимальный разброс в нуле, когда aimController.Progress равен 0
        float maxDeviationDegrees = 2f; // Максимальный разброс при aimController.Progress равен 1

        Debug.Log("aimController.Progress: " + aimController.Progress);
            
        // Вычислите текущий разброс, учитывая aimController.Progress
        float currentDeviationDegrees = Mathf.Lerp(minDeviationDegrees, maxDeviationDegrees, aimController.Progress);

        // Если aimController.Progress равен 0, устанавливает текущий разброс в 0 градусов
        if (aimController.Progress == 0)
        {
            currentDeviationDegrees = 0;
        }

        // Преобразуем  градусы в радианы
        float currentDeviationRadians = currentDeviationDegrees * Mathf.Deg2Rad;

        // Вычисляем случайный разброс в радианах
        float randomX = Random.Range(-currentDeviationRadians, currentDeviationRadians);
        float randomY = Random.Range(-currentDeviationRadians, currentDeviationRadians);
        float randomZ = Random.Range(-currentDeviationRadians, currentDeviationRadians);

        Vector3 deviation = new Vector3(randomX, randomY, randomZ);

     //   Debug.Log("Deviation: " + deviation);
        return deviation;
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
        tower.rotation = Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((GlobalPosition - tower.position).normalized,
                tower.up), horizontalRotationSpeed * Time.deltaTime);

        
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
