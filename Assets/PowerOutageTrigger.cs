using System.Collections;
using UnityEngine;

public class PowerOutageTrigger : MonoBehaviour
{
    [Header("Lights")]
    public Light[] lights;                 // all Light
    public Renderer[] emissiveRenderers; 

    [Header("Flicker Settings")]
    public int flickerCount = 4;
    public float flickerInterval = 0.15f;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(PowerOutageSequence());
    }

    IEnumerator PowerOutageSequence()
    {
        // 1. lights flicker
        for (int i = 0; i < flickerCount; i++)
        {
            SetLights(false);
            yield return new WaitForSeconds(flickerInterval);

            SetLights(true);
            yield return new WaitForSeconds(flickerInterval);
        }

        // 2. power goes off
        SetLights(false);

        // 3. show new dialogue
        yield return new WaitForSeconds(0.5f);

        dialogueUI.StartDialogue(new string[]
        {
            "What happened...",
            "The power is off, I need to turn out the lights first.",
            "Where is the electrical box?",
            "I remember seeing one at one of the corners..."
        });
    }

    void SetLights(bool on)
    {
        foreach (var light in lights)
        {
            if (light != null)
                light.enabled = on;
        }

        foreach (var rend in emissiveRenderers)
        {
            if (rend != null)
            {
                if (on)
                    rend.material.EnableKeyword("_EMISSION");
                else
                    rend.material.DisableKeyword("_EMISSION");
            }
        }
    }
}