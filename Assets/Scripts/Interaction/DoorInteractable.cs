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
        RequiresCardReaderAccess
    }

    [Header("Requirement")]
    public RequirementType requirement = RequirementType.None;

    // === Item Requirement ===
    public string requiredItemId = "student card";
    public string failDialogue = "I forget to pickup my student ID card";

    // === Card Reader Requirement ===
    [Header("Card Reader")]
    public CardReaderInteractable cardReader;  // ✅ 新增：拖拽门旁读卡器进来
    public string needSwipeDialogue = "I need to use the card reader first.";

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

        if (requirement == RequirementType.RequiresCardReaderAccess)
        {
            if (cardReader == null || !cardReader.accessGranted)
            {
                dialogueUI?.StartDialogue(new string[] { needSwipeDialogue });
                return;
            }
        }

        // ===== 条件通过，开门 =====
        Quaternion target = isOpen ? closedRot : openRot;
        StartCoroutine(AnimateDoor(target));
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
        isOpen = !isOpen;
        isAnimating = false;
    }
}