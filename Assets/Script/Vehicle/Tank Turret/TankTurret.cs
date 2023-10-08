using UnityEngine;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : Turret
{
    private TrackTank tank;

    [SerializeField] private UICannonAim cannonAim;

    
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

    private float previousTowerAngle;
    
    private float previousMaskAngle;
    
    private float currentMaskAngle;

    private float currentTowerAngle;
    
    private float timeEqual = 0f; // Время, в течение которого значения были равны
    private bool isChangingSize = false; // Флаг для отслеживания изменения размера

    private float smoothingFactor = 0.9f; // Здесь можно настроить степень сглаживания
    
    float epsilon = 0.001f;

    private float maskCurrentAngel;
    
    private float timeSinceLastMovement = 0f;

    private Rigidbody tankRigidbody;

    [SerializeField] private float angularSpeedThreshold = 5f; // Пороговое значение угловой скорости
    
    private void Start()
    {
        tank = GetComponent<TrackTank>();
        tankRigidbody = tank.GetComponent<Rigidbody>();

        previousTowerAngle = GetTowerAngle();
    }

    protected override void Update()
    {
        base.Update();

        ControlTurretAim(); 
    //    InvokeRepeating("CheckTowerAngle", 0f, 2f);
       CheckTowerAngle();
    }

    protected override void OnFire()
    {
        base.OnFire();

        var projectile = Instantiate(SelectedProjectile.ProjectilePrefab); // var - тот же.
        
        Debug.Log($"projectile + {projectile.name}");
        
        projectile.Owner = tank.Owner;
        projectile.SetProperties(SelectedProjectile);
        
    
        projectile.transform.position = launchPoint.position;
        projectile.transform.forward = launchPoint.forward;
 

        FireSfx();
    }

    private void FireSfx()
    {
        fireSound.Play();
        mazzel.Play();

        tankRigidbody.AddForceAtPosition(-mask.forward * foreceRecoil, mask.position, ForceMode.Impulse);
    }

    
    private void CheckTowerAngle()
    { 
        // Добавьте проверку движения башни и маски
        if (IsTurretMoving() ) //|| IsMaskMoving()
        {
            cannonAim.StartChangingSize(false);
        }
        else
        {
            cannonAim.StartChangingSize(true);
            Debug.Log("Turret and mask are not moving for a while.");
        }
    }

    // Добавьте методы для определения движения башни и маски
    private bool IsTurretMoving()
    {
        currentTowerAngle = GetTowerAngle();

        previousTowerAngle = Mathf.Lerp(previousTowerAngle, currentTowerAngle, smoothingFactor);

        if (Mathf.Abs(previousTowerAngle - currentTowerAngle) < epsilon)
        {
            return true;
        }
        return false;
    }

    private bool IsMaskMoving()
    {
        currentMaskAngle = GetMaskAngle();

        previousMaskAngle = Mathf.Lerp(previousMaskAngle, currentMaskAngle, smoothingFactor);
        
        
        if (Mathf.Abs(previousMaskAngle - currentMaskAngle) < epsilon)
        {
            return true;
        }
        return false;
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
    //    currentMaskAngle = GetMaskAngle();
   //     previousMaskAngle = Mathf.Lerp(previousMaskAngle, currentMaskAngle, smoothingFactor);
        
        
        
     //   Debug.Log(mask.position + "Mask");
        mask.localRotation = Quaternion.identity;

        LocalPosition = mask.InverseTransformPoint(tank.NetAimPoint);
        LocalPosition.x = 0;
        GlobalPosition = tower.TransformPoint(LocalPosition);


        float targetAngel = -Vector3.SignedAngle((GlobalPosition - mask.position).normalized, mask.forward, mask.right);
        targetAngel = Mathf.Clamp(targetAngel, maxTopAngel, maxBottomAngel);

        maskCurrentAngel = Mathf.MoveTowards(maskCurrentAngel, targetAngel, Time.deltaTime * verticalRotationSpeed);
        mask.localRotation = Quaternion.Euler(maskCurrentAngel, 0, 0);
        
    }
    
    private float GetTowerAngle()
    {
        // Возвращает угол башни относительно направления вперед танка
        return Vector3.SignedAngle(tower.forward, tank.NetAimPoint - tower.position, Vector3.up);
    }

    private float GetMaskAngle()
    {
        // Возвращает угол маски относительно направления вперед танка
        return Vector3.SignedAngle(mask.forward, tank.NetAimPoint - mask.position, Vector3.up);
    }
}
