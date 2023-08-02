using UnityEngine;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : MonoBehaviour
{
    private TrackTank tank;

    [SerializeField] private Transform aim;

    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;

    [SerializeField] protected float horizontalRotationSpeed;
    [SerializeField] protected float verticalRotationSpeed;


    [SerializeField] protected float maxTopAngel;
    [SerializeField] protected float maxBottomAngel;


    private float maskCurrentAngel;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void Update()
    {
        ControlTurretAim();
    }

    private void ControlTurretAim()
    {
        //Tower
        Vector3 LocalPosition = tower.InverseTransformPoint(aim.position);
        LocalPosition.y = 0;
        Vector3 GlobalPosition = tower.TransformPoint(LocalPosition);
        tower.rotation = Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((GlobalPosition - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);

        //Mask
        mask.localRotation = Quaternion.identity;

        LocalPosition = mask.InverseTransformPoint(aim.position);
        LocalPosition.x = 0;
        GlobalPosition = tower.TransformPoint(LocalPosition);


        float targetAngel = -Vector3.SignedAngle((GlobalPosition - mask.position).normalized, mask.forward, mask.right);
        targetAngel = Mathf.Clamp(targetAngel, maxTopAngel, maxBottomAngel);

        maskCurrentAngel = Mathf.MoveTowards(maskCurrentAngel, targetAngel, Time.deltaTime * verticalRotationSpeed);
        mask.localRotation = Quaternion.Euler(maskCurrentAngel, 0, 0);
    }
}
