using UnityEngine;

public class TankTrackTextureMovement : MonoBehaviour
{
    private TrackTank tank;

    [SerializeField] private Renderer leftTrackRenderer;
    [SerializeField] private Renderer rightTrackRenderer;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float modefier;


    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void FixedUpdate()
    {
        float speed = tank.LeftWheelRmp / 60.0f * modefier * Time.fixedDeltaTime;
        leftTrackRenderer.material.SetTextureOffset("_MainTex", leftTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);

        speed = tank.RighttWheelRmp / 60.0f * modefier * Time.fixedDeltaTime;
        rightTrackRenderer.material.SetTextureOffset("_MainTex", rightTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);
    }
}
