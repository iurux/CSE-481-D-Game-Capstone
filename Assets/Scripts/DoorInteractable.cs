using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [Header("Door Rotation")]
    public Transform doorToRotate;        
    public float openAngleY = 90f; 
    public float openSpeed = 2f;

    [Header("Requirement Type 1: Requires Item")]
    public bool requiresItem = true;
    public string requiredItemId = "student card";
    public string failDialogue = "i forget to pickup my student card";

    private bool isOpen = false;
    private bool isAnimating = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    private void Awake()
    {
        if (doorToRotate == null) doorToRotate = transform;
        closedRot = doorToRotate.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngleY, 0f);
    }

    public void TryInteract(InventorySimple inv, DialogueUI dialogueUI)
    {
        if (isAnimating) return;

        // 检查限制：需要 student card
        if (requiresItem)
        {
            if (inv == null || !inv.Has(requiredItemId))
            {
                if (dialogueUI != null)
                    dialogueUI.StartDialogue(new string[] { failDialogue });
                return;
            }
        }

        // 通过限制：开门/关门（你只想开门就只保留 OpenDoor 也行）
        if (!isOpen) StartCoroutine(AnimateDoor(openRot));
        else StartCoroutine(AnimateDoor(closedRot));
    }

    private IEnumerator AnimateDoor(Quaternion target)
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
        isOpen = (target == openRot);
        isAnimating = false;
    }
}
