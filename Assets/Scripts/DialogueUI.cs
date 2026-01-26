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
        // ⚠️ 脚本始终启用，只隐藏 UI
        panel.SetActive(false);
        continueHint.gameObject.SetActive(false);
    }

    /// <summary>
    /// 外部唯一需要调用的方法
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
        yield return null; // 确保 UI 刷新

        // 保证文本可见
        Color tc = dialogueText.color;
        tc.a = 1f;
        dialogueText.color = tc;

        foreach (string line in lines)
        {
            dialogueText.text = line;

            // 清掉本帧输入
            yield return null;

            yield return WaitForAdvance();
        }

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