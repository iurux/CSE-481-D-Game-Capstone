using UnityEngine;

public class ScreenFlicker : MonoBehaviour
{
    public Renderer screenRenderer;
    public float flickerSpeed = 0.08f;
    public float minIntensity = 0.3f;
    public float maxIntensity = 1.2f;

    Material mat;

    void Start()
    {
        mat = screenRenderer.material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            Mathf.PingPong(Time.time / flickerSpeed, 1f)
        );

        mat.SetColor("_EmissionColor", Color.white * intensity);
    }
}