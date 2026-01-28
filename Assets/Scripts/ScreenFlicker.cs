using UnityEngine;

public class ScreenFlicker : MonoBehaviour
{
    public Renderer screenRenderer;   // 拖 Quad 的 Renderer

    [Header("Emission Color")]
    public Color emissionColor = new Color(0.2f, 0.6f, 1f, 1f); // 蓝光

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
        if (screenRenderer == null)
        {
            Debug.LogError("ScreenFlicker: screenRenderer is null");
            enabledFlicker = false;
            return;
        }

        // 用实例材质，避免改到全局共享材质
        mat = screenRenderer.material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!enabledFlicker || mat == null) return;

        float min = alertMode ? alertMin : normalMin;
        float max = alertMode ? alertMax : normalMax;
        float speed = alertMode ? alertSpeed : normalSpeed;

        float intensity = Mathf.Lerp(min, max, Mathf.PingPong(Time.time / speed, 1f));

        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }

    //public void Intensify() => alertMode = true;
    //public void CalmDown() => alertMode = false;

    //public void StopFlicker()
    //{
    //    enabledFlicker = false;
    //    if (mat != null) mat.SetColor("_EmissionColor", Color.black);
    //}
}
