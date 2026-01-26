using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OpeningScene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject menuPanel;
    public Image blinkPanel;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;

    [Header("Post-Processing References")]
    public Volume globalVolume;
    private DepthOfField dof;

    [Header("Animation Settings")]
    public float sleepDuration = 2.0f;
    public int blinkCount = 3;
    public float blinkSpeed = 2.0f;

    private const float BLURRY_VALUE = 0.001f;
    private const float CLEAR_VALUE = 30f;
    private const float MAX_BLUR_RADIUS = 1.5f;

    [Header("Stand Up After Blinking")]
    public MonoBehaviour firstPersonController;
    public Transform cameraRig;

    public float standUpDuration = 1.2f;
    public Vector3 standUpFinalLocalPos = new Vector3(0f, 0.373f, 0f);
    public Vector3 standUpFinalLocalEuler = Vector3.zero;
    public AnimationCurve standUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool disableControllerDuringStandUp = true;

    [Header("Opening Thoughts")]
    [TextArea(2, 3)]
    public string[] openingThoughts =
    {
        "Ehhhh.... What time is it now?",
        "Already 2 am...",
        "The lab is due tomorrow but still a ton of bugs to fix...",
        "Okay I need some coffee refresher.",
        "I remember SnackOverflow has some."
    };

    private void Start()
    {
        menuPanel.SetActive(true);
        blinkPanel.gameObject.SetActive(false);

        if (globalVolume.profile.TryGet(out dof))
        {
            dof.active = true;
            dof.mode.value = DepthOfFieldMode.Gaussian;

            dof.gaussianStart.overrideState = true;
            dof.gaussianEnd.overrideState = true;
            dof.gaussianMaxRadius.overrideState = true;

            dof.gaussianStart.value = 0f;
            dof.gaussianEnd.value = BLURRY_VALUE;
            dof.gaussianMaxRadius.value = MAX_BLUR_RADIUS;
        }
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(PlayOpeningSequence());
    }

    private IEnumerator PlayOpeningSequence()
    {
        // Step 1: Black screen
        menuPanel.SetActive(false);
        blinkPanel.gameObject.SetActive(true);
        SetBlinkAlpha(1f);

        yield return new WaitForSeconds(sleepDuration);

        // Make sure DialogueUI / Canvas / Awake all complete
        yield return null;

        if (dialogueUI != null && openingThoughts.Length > 0)
        {
            dialogueUI.StartDialogue(openingThoughts);
        }

        // Step 2: Blinking
        for (int i = 0; i < blinkCount; i++)
        {
            float progress = (float)(i + 1) / blinkCount;
            float dramaticProgress = Mathf.Pow(progress, 4f);

            float targetBlur = Mathf.Lerp(BLURRY_VALUE, CLEAR_VALUE, dramaticProgress);

            if (dof != null)
            {
                dof.gaussianMaxRadius.value =
                    Mathf.Lerp(5.0f, 0.5f, dramaticProgress);
            }

            // Open eyes
            yield return StartCoroutine(
                FadeBlink(1f, 0f, blinkSpeed * 0.5f, targetBlur)
            );

            if (i < blinkCount - 1)
            {
                yield return new WaitForSeconds(0.5f);

                // Close eyes
                yield return StartCoroutine(
                    FadeBlink(0f, 1f, blinkSpeed * 0.5f, targetBlur)
                );

                yield return new WaitForSeconds(0.2f);
            }
        }

        // Step 3: Fully awake
        if (dof != null)
            dof.active = false;

        blinkPanel.gameObject.SetActive(false);

        // Step 4: Stand up AFTER consciousness
        yield return StartCoroutine(StandUpSequence());
    }

    private IEnumerator FadeBlink(float startAlpha, float endAlpha, float duration, float targetBlur)
    {
        float elapsedTime = 0f;
        float startBlurValue = dof != null ? dof.gaussianEnd.value : BLURRY_VALUE;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            SetBlinkAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            if (dof != null && endAlpha == 0f)
            {
                dof.gaussianEnd.value =
                    Mathf.Lerp(startBlurValue, targetBlur, t);
            }

            yield return null;
        }

        SetBlinkAlpha(endAlpha);
    }

    private void SetBlinkAlpha(float alpha)
    {
        Color c = blinkPanel.color;
        c.a = alpha;
        blinkPanel.color = c;
    }

    private IEnumerator StandUpSequence()
    {
        Transform rig = cameraRig != null
            ? cameraRig
            : Camera.main?.transform;

        if (rig == null) yield break;

        Vector3 startPos = rig.localPosition;
        Quaternion startRot = rig.localRotation;

        Quaternion endRot = Quaternion.Euler(standUpFinalLocalEuler);

        if (disableControllerDuringStandUp && firstPersonController != null)
            firstPersonController.enabled = false;

        float t = 0f;
        while (t < standUpDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / standUpDuration);
            float eased = standUpCurve.Evaluate(p);

            rig.localPosition =
                Vector3.Lerp(startPos, standUpFinalLocalPos, eased);
            rig.localRotation =
                Quaternion.Slerp(startRot, endRot, eased);

            yield return null;
        }

        rig.localPosition = standUpFinalLocalPos;
        rig.localRotation = endRot;

        if (disableControllerDuringStandUp && firstPersonController != null)
            firstPersonController.enabled = true;
    }
}