using UnityEngine;
using UnityEngine.UI;

public class UIImageChanger : MonoBehaviour
{
    [SerializeField] private float repeatRate = 0.5f;
    [SerializeField] private float purdueTime = 0f;

    private string methodName = "ChangeImage";
    public Image uiImage;
    public Sprite[] images; // Массив спрайтов, которые вы хотите использовать.

    private int currentIndex = 0;
    
    

    private void Start()
    {
        InvokeRepeating(methodName, purdueTime, repeatRate); // Вызываем метод ChangeImage каждую секунду.
    }

    private void ChangeImage()
    {
        if (uiImage != null && images.Length > 0)
        {
            // Устанавливаем новый источник изображения.
            uiImage.sprite = images[currentIndex];

            // Переходим к следующему изображению.
            currentIndex = (currentIndex + 1) % images.Length;
        }
    }
}