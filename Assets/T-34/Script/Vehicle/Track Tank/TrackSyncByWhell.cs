using UnityEngine;

[System.Serializable]
public class WheelSyncPoint
{
    public Transform bone;
    public Transform mesh;
    public Vector3 offset;
}

public class TrackSyncByWhell : MonoBehaviour
{
    [SerializeField] private WheelSyncPoint[] SyncPoints;

    private void Start()
    {
        for (int i = 0; i < SyncPoints.Length; i++)
        {
            SyncPoints[i].offset = SyncPoints[i].bone.localPosition - SyncPoints[i].mesh.localPosition;
            Debug.Log(SyncPoints[i].offset);
        }
    }


    private void Update()
    {
        for (int i = 0; i < SyncPoints.Length; i++)
        {
            SyncPoints[i].bone.localPosition = SyncPoints[i].mesh.localPosition + SyncPoints[i].offset;
        }
    }
}
