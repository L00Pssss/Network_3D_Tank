using UnityEngine;

[RequireComponent (typeof(Camera))]
public class VehicleCamera : MonoBehaviour
{
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private Vector3 offset;


    [Header("Sencetive Limit")]
    [SerializeField] private float rotateSensetive;
    [SerializeField] private float scrollSensetive;

    [Header("Rotation Limit")]
    [SerializeField] private float maxVericalAngel;
    [SerializeField] private float minVerticalAngel;


    [Header("Distance")]
    [SerializeField] private float distance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float distanceOffsetFromCollisionHit;
    [SerializeField] private float distanceLerpRate;

    private new Camera camera;


    private Vector2 rotationContorol;

    private float deltaRotationX;
    private float deltaRotationY;

    private float currentDistance;


    private void Start()
    {
        camera = GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        UpdateControl();

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        deltaRotationX += rotationContorol.x * rotateSensetive;
        deltaRotationY += rotationContorol.y * -rotateSensetive;

        deltaRotationY = ClampAngel(deltaRotationY, minVerticalAngel, maxVericalAngel);


        Quaternion finalRotatoin = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
        Vector3 finalPositon = vehicle.transform.position - (finalRotatoin * Vector3.forward * distance);
        finalPositon = AddLocalOffset(finalPositon);

        float targetDistance = distance;

        RaycastHit hit;


        Debug.DrawLine(vehicle.transform.position + new Vector3(0, offset.y, 0), finalPositon, Color.red);

        if (Physics.Linecast(vehicle.transform.position + new Vector3(0, offset.y, 0), finalPositon, out hit) == true)
        {
            float distanceTohit = Vector3.Distance(vehicle.transform.position + new Vector3(0, offset.y, 0), hit.point);

            if (hit.transform != vehicle)
            {
                if (distanceTohit < distance)
                    targetDistance = distanceTohit - distanceOffsetFromCollisionHit;
            }
        }

        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

        finalPositon = vehicle.transform.position - (finalRotatoin * Vector3.forward * currentDistance);

        transform.rotation = finalRotatoin;
        transform.position = finalPositon;
        transform.position = AddLocalOffset(transform.position);
    }

    private void UpdateControl()
    {
        rotationContorol = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        distance += -Input.mouseScrollDelta.y * scrollSensetive;
    }
    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, offset.y, 0);
        result += transform.right * offset.x;
        result += transform.forward * offset.z;
        return result;
    }

    private float ClampAngel(float angel, float min, float max)
    {
        if (angel < -360)
        {
            angel += 360;
        }
        if (angel > 360)
        {
            angel -= 360;
        }

        return Mathf.Clamp(angel, min, max);
    }

    public void SetTarget(Vehicle target)
    {
        vehicle = target;
    }

}
