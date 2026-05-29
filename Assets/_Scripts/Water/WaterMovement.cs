using UnityEngine;

using UnityEngine.UI;

public class AnimGif : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Texture2D[] frames;
    [SerializeField] private float fps = 10f;

    [Header("Tiling")]
    [SerializeField] private Vector2 textureTiling = Vector2.one;
    [SerializeField] private Vector2 textureOffset = Vector2.zero;

    [Header("Transparency")]
    [Range(0f, 1f)]
    [SerializeField] private float alpha = 1f;

    private Material mat;
    private RawImage rawImage;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            // Creates instance so original material is not modified
            mat = renderer.material;
        }

        rawImage = GetComponent<RawImage>();

        ApplySettings();
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        int index = (int)(Time.time * fps) % frames.Length;

        // 3D Object Renderer
        if (mat != null)
        {
            mat.mainTexture = frames[index];
        }

        // UI RawImage
        if (rawImage != null)
        {
            rawImage.texture = frames[index];
        }

        ApplySettings();
    }

    private void ApplySettings()
    {
        // Apply to Material
        if (mat != null)
        {
            mat.mainTextureScale = textureTiling;
            mat.mainTextureOffset = textureOffset;

            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }

        // Apply to UI RawImage
        if (rawImage != null)
        {
            rawImage.uvRect = new Rect(
                textureOffset.x,
                textureOffset.y,
                textureTiling.x,
                textureTiling.y
            );

            Color color = rawImage.color;
            color.a = alpha;
            rawImage.color = color;
        }
    }
}