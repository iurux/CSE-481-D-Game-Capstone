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

        if (boxIndicator != null && lightsOnMaterial != null)
            boxIndicator.material = lightsOnMaterial;
    }

    private void ToggleLights(bool isActive)
    {
        foreach (GameObject lightObj in roomLights)
        {
            if (lightObj != null)
                lightObj.SetActive(isActive);
        }
    }
}