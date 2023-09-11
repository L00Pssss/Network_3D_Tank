using UnityEngine;

public class SizeMap : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    public Vector2 Size
    {
        get { return size; }

    }

    public Vector3 GetNormalPositon(Vector3 positon)
    {
        return new Vector3(positon.x / (size.x * 0.5f), 0, positon.z / (size.y * 0.5f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 0, size.y));
    }
}
