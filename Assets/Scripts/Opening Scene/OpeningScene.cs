using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Required for URP Post-Processing

public class OpeningScene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject menuPanel;        // The main menu UI group
    public Image blinkPanel;            // The black image for blinking effect

    [Header("Post-Processing References")]
    public Volume globalVolume;         // Reference to the Global Volume in the scene
    private DepthOfField dof;           // Reference to the Depth Of Field override

    [Header("Animation Settings")]
    public float sleepDuration = 2.0f;  // How long the screen stays dark
    public int blinkCount = 3;          // How many times the player blinks
    public float blinkSpeed = 2.0f;     // Speed of the blink animation
    private const float BLURRY_VALUE = 0.001f; // Vision is clear only up to 0.01m (Very Blurry)
    private const float CLEAR_VALUE = 30f;  // Vision is clear up to 50m (Clear)
    private const float MAX_BLUR_RADIUS = 1.5f; // Increase this for stronger blur intensity

    [Header("Stand Up After Blinking")]
    public MonoBehaviour firstPersonController;
    public Transform cameraRig; 

    public float standUpDuration = 1.2f; 
    public Vector3 standUpFinalLocalPos = new Vector3(0f, 0.373f, 0f);
    public Vector3 standUpFinalLocalEuler = new Vector3(0f, 0f, 0f);
    public AnimationCurve standUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool disableControllerDuringStandUp = true;


    private void Start()
    {
        // 1. Initialize UI
        menuPanel.SetActive(true);      // Show Menu
        blinkPanel.gameObject.SetActive(false); // Hide Black Screen initially

        // 2. Initialize Post-Processing (Get DOF component)
        if (globalVolume.profile.TryGet(out dof))
        {
            dof.active = true;
            dof.mode.value = DepthOfFieldMode.Gaussian;

            // Ensure the script has permission to override these values
            dof.gaussianStart.overrideState = true;
            dof.gaussianEnd.overrideState = true;
            dof.gaussianMaxRadius.overrideState = true;

            // 1. MUST start with BLURRY_DISTANCE
            dof.gaussianStart.value = 0f;
            dof.gaussianEnd.value = BLURRY_VALUE; 
            
            // 2. Increase blur intensity via code
            dof.gaussianMaxRadius.value = MAX_BLUR_RADIUS;
        }
    }

    // Connect this method to your Start Button's OnClick event
    public void OnStartButtonClicked()
    {
        StartCoroutine(PlayOpeningSequence());
    }

    private IEnumerator PlayOpeningSequence()
    {
        // Step 1: Hide Menu and Show Black Screen
        menuPanel.SetActive(false);
        blinkPanel.gameObject.SetActive(true);
        
        // Ensure screen is pitch black
        SetBlinkAlpha(1f);

        // Step 2: Simulate sleeping/unconscious state (Black screen duration)
        yield return new WaitForSeconds(sleepDuration);

        // Step 3: Blinking Effect (Waking up)
        // Loop through the blink count
        for (int i = 0; i < blinkCount; i++)
        {
            // Calculate blur amount: gradually goes from 5 to 0
            float progress = (float)(i + 1) / blinkCount;
            float dramaticProgress = Mathf.Pow(progress, 4.0f);

            // Gradually increase the 'End' value to make it clear
            // From 0.1 (Blurry) to 1.0 (Clear)
            float targetBlur = Mathf.Lerp(BLURRY_VALUE, CLEAR_VALUE, dramaticProgress);

            if (dof != null)
            {
                dof.gaussianMaxRadius.value = Mathf.Lerp(5.0f, 0.5f, dramaticProgress);
            }
            
            // A. Open Eyes (Fade Black -> Transparent)
            yield return StartCoroutine(FadeBlink(1f, 0f, blinkSpeed * 0.5f, targetBlur));

            // Reduce blur slightly each time eyes open
            /*if (dof != null)
            {
                float currentBlur = Mathf.Lerp(0.1f, CLEAR_VALUE, progress);
                dof.gaussianEnd.value = currentBlur;
            }*/

            // Wait a moment with eyes open (unless it's the last blink)
            if (i < blinkCount - 1)
            {
                yield return new WaitForSeconds(0.5f);

                // B. Close Eyes (Fade Transparent -> Black)
                yield return StartCoroutine(FadeBlink(0f, 1f, blinkSpeed * 0.5f, targetBlur)); // Closing eyes is faster
                
                yield return new WaitForSeconds(0.2f); // Keep eyes closed briefly
            }
        }

        // Step 4: Sequence Finished - Clear Vision
        if (dof != null)
        {
            dof.active = false; // Disable blur completely
        }

        blinkPanel.gameObject.SetActive(false); // Disable the black panel

        yield return StartCoroutine(StandUpSequence()); // stand up and look up
    }

    // Helper coroutine to fade the alpha of the black panel
    private IEnumerator FadeBlink(float startAlpha, float endAlpha, float duration, float targetBlur)
    {
        float elapsedTime = 0f;
        float startBlurValue = (dof != null) ? dof.gaussianEnd.value : BLURRY_VALUE;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // float t = elapsedTime / duration;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            // Update Eyelids
            SetBlinkAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            // Update Blur (Vision clears as eyelids open)
            if (dof != null)
            {
                // Only change blur when opening eyes (transitioning to endAlpha 0)
                if (endAlpha == 0)
                {
                    dof.gaussianEnd.value = Mathf.Lerp(startBlurValue, targetBlur, t);
                }
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
        Transform rig = cameraRig != null ? cameraRig : (Camera.main != null ? Camera.main.transform : null);
        if (rig == null) yield break;

        Vector3 startPos = rig.localPosition;
        Quaternion startRot = rig.localRotation;

        Vector3 endPos = standUpFinalLocalPos;
        Quaternion endRot = Quaternion.Euler(standUpFinalLocalEuler);

        // disable camera controller
        if (disableControllerDuringStandUp && firstPersonController != null)
            firstPersonController.enabled = false;

        float t = 0f;
        while (t < standUpDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / standUpDuration);
            float eased = standUpCurve != null ? standUpCurve.Evaluate(p) : p;

            rig.localPosition = Vector3.Lerp(startPos, endPos, eased);
            rig.localRotation = Quaternion.Slerp(startRot, endRot, eased);

            yield return null;
        }

        rig.localPosition = endPos;
        rig.localRotation = endRot;

        // animation end, camera controller back
        if (disableControllerDuringStandUp && firstPersonController != null)
            firstPersonController.enabled = true;
    }

}
