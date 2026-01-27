using UnityEngine;

public class ScreenFlicker : MonoBehaviour
{
    public Renderer screenRenderer;

    [Header("Normal Flicker")]
    public float normalMin = 0.3f;
    public float normalMax = 0.6f;
    public float normalSpeed = 0.08f;

    [Header("Alert Flicker")]
    public float alertMin = 0.8f;
    public float alertMax = 1.6f;
    public float alertSpeed = 0.04f;

    Material mat;
    bool alertMode = false;
    bool enabledFlicker = true;

    void Start()
    {
        mat = screenRenderer.material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!enabledFlicker) return;

        float min = alertMode ? alertMin : normalMin;
        float max = alertMode ? alertMax : normalMax;
        float speed = alertMode ? alertSpeed : normalSpeed;

        float intensity = Mathf.Lerp(
            min,
            max,
            Mathf.PingPong(Time.time / speed, 1f)
        );

        mat.SetColor("_EmissionColor", Color.white * intensity);
    }

    // ===== public control =====
    public void Intensify()
    {
        alertMode = true;
    }

    public void CalmDown()
    {
        alertMode = false;
    }

    public void StopFlicker()
    {
        enabledFlicker = false;
        mat.SetColor("_EmissionColor", Color.black);
    }
}