using System;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleDimensions : MonoBehaviour
{
    [SerializeField] protected Transform[] points;

    [SerializeField] private Transform[] priorityFirePoints;

    private Vehicle vehicle;
    public Vehicle Vehicle => vehicle;

    private RaycastHit[] hits = new RaycastHit[10];
    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    public bool IsVisibleFromPoint(Transform source, Vector3 point, Color color)
    {
        bool visible = true;

        for (int i = 0; i < points.Length; i++)
        {
         //    Debug.DrawLine(point, points[i].position, color);

            int lenghtRaycastHit  = Physics.RaycastNonAlloc(point, (points[i].position - point).normalized, hits,
                Vector3.Distance(point, points[i].position));

            visible = true;
            for (int j = 0; j < lenghtRaycastHit; j++)
            {
                if (hits[j].collider.transform.root == source || hits[j].collider.transform.root == transform.root)

                    visible = false;
            }

            if (visible == true)
                return visible;
        }
        return false;
    }

    public Transform GetPriorityFirePoint()
    {
        if (priorityFirePoints.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, priorityFirePoints.Length);
            return priorityFirePoints[randomIndex];
        }
        else
        {
            Debug.LogError("Null Points");
            return null;
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(points == null) return;
        
        Gizmos.color = Color.blue;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i].position, 0.2f);
        }
    }
    #endif
}
