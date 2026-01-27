using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [Header("Door Rotation")]
    public Transform doorToRotate;
    public float openAngleY = 90f;
    public float openSpeed = 2f;

    public enum RequirementType
    {
        None,
        RequiresItem,
        RequiresMazeSolved
    }

    [Header("Requirement")]
    public RequirementType requirement = RequirementType.None;

    // === Item Requirement ===
    public string requiredItemId = "student card";
    public string failDialogue = "i forget to pickup my student card";

    // === Maze Requirement ===
    [TextArea]
    public string[] mazeLockedDialogue =
    {
        "I don't have access.",
        "I need to grant access to my student ID first.",
        "I think I can use the computer over there."
    };

    [Header("Hint Target")]
    public ScreenFlicker computerScreen;   // 👈 新增

    bool isOpen = false;
    bool isAnimating = false;

    Quaternion closedRot;
    Quaternion openRot;

    void Awake()
    {
        if (doorToRotate == null) doorToRotate = transform;
        closedRot = doorToRotate.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngleY, 0f);
    }

    public void TryInteract(InventorySimple inv, DialogueUI dialogueUI)
    {
        if (isAnimating) return;

        // ===== 条件检查 =====
        if (requirement == RequirementType.RequiresItem)
        {
            if (inv == null || !inv.Has(requiredItemId))
            {
                dialogueUI?.StartDialogue(new string[] { failDialogue });
                return;
            }
        }

        if (requirement == RequirementType.RequiresMazeSolved)
        {
            if (!MazeProgress.mazeSolved)
            {
                dialogueUI?.StartDialogue(mazeLockedDialogue);
                computerScreen?.Intensify();   // ✅ 这里改了
                return;
            }
        }

        // ===== 条件通过，开门 =====
        StartCoroutine(AnimateDoor(openRot));
    }

    IEnumerator AnimateDoor(Quaternion target)
    {
        isAnimating = true;

        Quaternion start = doorToRotate.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorToRotate.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        doorToRotate.localRotation = target;
        isOpen = true;
        isAnimating = false;
    }
}