using System;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleDimensions : MonoBehaviour
{
    [SerializeField] protected Transform[] points;

    private Vehicle vehicle;
    public Vehicle Vehicle => vehicle;
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

            RaycastHit[]  hits = Physics.RaycastAll(point, (points[i].position - point).normalized,
                Vector3.Distance(point, points[i].position));

            visible = true;
            for (int j = 0; j < hits.Length; j++)
            {
                if(hits[j].collider.transform.root == source)
                    continue;
                
                if(hits[j].collider.transform.root == transform.root)
                    continue;

                visible = false;
            }

            if (visible == true)
                return visible;
        }
        return false;
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
