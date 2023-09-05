using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Vector2 movementDirection;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float lifeTime;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
    }
}
