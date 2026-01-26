using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public TMP_Text continueHint;

    [Header("Input")]
    public Key advanceKey = Key.Space;

    [Header("Blink")]
    public float blinkSpeed = 0.6f;

    bool waitingForInput;
    Coroutine blinkRoutine;
    Coroutine dialogueRoutine;

    void Awake()
    {
        panel.SetActive(false);
        continueHint.gameObject.SetActive(false);
    }

    /// <summary>
    /// The only public method
    /// </summary>
    public void StartDialogue(string[] lines)
    {
        // 防止重复启动
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine = StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        panel.SetActive(true);
        yield return null; // make sure UI refresh

        // make sure text visible
        Color tc = dialogueText.color;
        tc.a = 1f;
        dialogueText.color = tc;

        for (int i = 0; i < lines.Length; i++)
        {
            dialogueText.text = lines[i];
            yield return null;

            // if last sentence, just terminated it
            if (i < lines.Length - 1)
                yield return WaitForAdvance();
            else
                yield return new WaitForSeconds(3.0f); // show last sentence
        }

        // Initializing UI
        dialogueText.text = "";
        continueHint.gameObject.SetActive(false);
        panel.SetActive(false);
        dialogueRoutine = null;
    }

    IEnumerator WaitForAdvance()
    {
        waitingForInput = true;
        continueHint.gameObject.SetActive(true);

        blinkRoutine = StartCoroutine(BlinkHint());

        while (waitingForInput)
        {
            if (
                (Keyboard.current != null &&
                 Keyboard.current[advanceKey].wasPressedThisFrame) ||
                (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame)
            )
            {
                waitingForInput = false;
            }

            yield return null;
        }

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        continueHint.gameObject.SetActive(false);
    }

    IEnumerator BlinkHint()
    {
        while (true)
        {
            continueHint.alpha = 1f;
            yield return new WaitForSeconds(blinkSpeed);
            continueHint.alpha = 0f;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}