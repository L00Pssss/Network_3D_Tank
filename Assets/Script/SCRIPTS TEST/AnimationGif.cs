using UnityEngine;
using UnityEngine.UI;

public class AnimationGif : MonoBehaviour
{
    [SerializeField] private Texture2D[] frames;

    private float framesPerSecond = 15f;

    private RawImage image = null;

    private Renderer render = null;

    private void Awake()
    {
        image = GetComponent<RawImage>();

        render = GetComponent<Renderer>();
    }


    private void Update()
    {
        float index = Time.time * framesPerSecond;

        index = index % frames.Length;

        if (render != null)
            render.material.mainTexture = frames[(int)index];
        else
            image.texture = frames[(int)index];
    }
}
