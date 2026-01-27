using UnityEngine;

public class PowerSystem : MonoBehaviour
{
    [Header("Environment")]
    public GameObject[] roomLights;        // Array of all lights in the room
    public MeshRenderer boxIndicator;      // The LED on the electricity box
    
    [Header("Visuals")]
    public Material lightsOnMaterial;      // Material for the box LED (Green)
    public Material lightsOffMaterial;     // Material for the box LED (Red)

    private void Start()
    {
        // Turn off all lights when the game starts
        // ToggleLights(false);
        
        if (boxIndicator != null && lightsOffMaterial != null)
            boxIndicator.material = lightsOffMaterial;
    }

    public void RestorePower()
    {
        ToggleLights(true);

        // 깜박임 로직이 있다면 찾아서 끕니다.
        foreach (GameObject lightObj in roomLights)
        {
            if (lightObj != null)
            {
                // ScreenFlicker 스크립트가 있다면 비활성화
                ScreenFlicker flicker = lightObj.GetComponent<ScreenFlicker>();
                if (flicker != null) flicker.enabled = false;
            
                // 혹시 다른 이름의 깜박임 스크립트가 있다면 그것도 꺼야 합니다.
            }
        }

        if (boxIndicator != null && lightsOnMaterial != null)
            boxIndicator.material = lightsOnMaterial;
    }

    private void ToggleLights(bool isActive)
    {
        foreach (GameObject lightObj in roomLights)
        {
            if (lightObj != null)
            {
                // 1. 오브젝트 자체를 활성화
                lightObj.SetActive(isActive);

                // 2. 팀원이 꺼버린 Light 부품을 찾아서 다시 켬
                Light lightComp = lightObj.GetComponent<Light>();
                if (lightComp != null)
                {
                    lightComp.enabled = isActive;
                }

                // 3. 팀원이 꺼버린 머티리얼 발광(Emission)을 다시 켬
                Renderer rend = lightObj.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (isActive)
                        rend.material.EnableKeyword("_EMISSION");
                    else
                        rend.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}