using UnityEngine;
using UnityEngine.UI;

public class AnimationGif : MonoBehaviour
{
    [SerializeField] private Texture2D[] frames;

    private float framesPerSecond = 15f;

    private RawImage image = null;

    private Renderer renderer = null;

    private void Awake()
    {
        image = GetComponent<RawImage>();

        renderer = GetComponent<Renderer>();
    }


    private void Update()
    {
        float index = Time.time * framesPerSecond;

        index = index % frames.Length;

        if (renderer != null)
            renderer.material.mainTexture = frames[(int)index];
        else
            image.texture = frames[(int)index];
    }
}
