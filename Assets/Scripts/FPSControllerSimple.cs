using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSControllerSimple : MonoBehaviour
{
    public Transform cam;

    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float lookSensitivity = 0.12f;
    public float gravity = -20f;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public Image interactPrompt;
    public TMP_Text keyText;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;

    [Header("Systems")]
    public InventorySimple inventory;
    public DialogueUI dialogueUI;

    float pitch;
    Vector3 vel;
    CharacterController cc;
    bool inventoryOpen;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        interactPrompt.gameObject.SetActive(false);
        inventoryPanel.SetActive(false);

        if (inventory == null)
            inventory = GetComponent<InventorySimple>();
    }

    void Update()
    {
        HandleInventoryToggle();

        if (inventoryOpen) return;

        HandleLook();
        HandleMove();
        HandleGravity();
        HandleInteractionRay();
    }

    // ================== Camera ==================
    void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mx = mouseDelta.x * lookSensitivity;
        float my = mouseDelta.y * lookSensitivity;

        transform.Rotate(0, mx, 0);
        pitch = Mathf.Clamp(pitch - my, -80f, 80f);
        cam.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    // ================== Movement ==================
    void HandleMove()
    {
        Vector2 move2 = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) move2.y += 1;
        if (Keyboard.current.sKey.isPressed) move2.y -= 1;
        if (Keyboard.current.dKey.isPressed) move2.x += 1;
        if (Keyboard.current.aKey.isPressed) move2.x -= 1;

        move2 = Vector2.ClampMagnitude(move2, 1f);
        Vector3 move = transform.right * move2.x + transform.forward * move2.y;
        cc.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (cc.isGrounded && vel.y < 0) vel.y = -1f;
        vel.y += gravity * Time.deltaTime;
        cc.Move(vel * Time.deltaTime);
    }

    // ================== Interaction ==================
    void HandleInteractionRay()
    {
        interactPrompt.gameObject.SetActive(false);

        Vector3 rayOrigin = cam.position + cam.forward * 0.2f;
        Ray ray = new Ray(rayOrigin, cam.forward);

        Debug.DrawRay(rayOrigin, cam.forward * interactDistance, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return;

        // ---------- Pickup ----------
        if (hit.collider.CompareTag("Pickup"))
        {
            ShowPrompt("F");

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                PickupItem p = hit.collider.GetComponentInParent<PickupItem>();

                string itemId = (p != null && !string.IsNullOrEmpty(p.itemId))
                    ? p.itemId
                    : hit.collider.name;

                Sprite icon = p != null ? p.icon : null;

                if (inventory != null)
                    inventory.Add(itemId, icon);

                Debug.Log("Picked up: " + itemId);
                Destroy(hit.collider.gameObject);
            }
        }
        // ---------- Interact ----------
        else if (hit.collider.CompareTag("Interact"))
        {
            ShowPrompt("E");

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("Interacted with: " + hit.collider.name);

                // ① Door
                DoorInteractable door =
                    hit.collider.GetComponentInParent<DoorInteractable>();
                if (door != null)
                {
                    door.TryInteract(inventory, dialogueUI);
                    return;
                }

                // ② Computer (Maze)
                ComputerInteract computer =
                    hit.collider.GetComponent<ComputerInteract>() ??
                    hit.collider.GetComponentInParent<ComputerInteract>();

                if (computer != null)
                {
                    computer.Interact();
                    return;
                }

                // ③ Fallback
                Debug.Log("No interactable script found on object.");
            }
        }
    }

    void ShowPrompt(string key)
    {
        interactPrompt.gameObject.SetActive(true);
        keyText.text = key;
    }

    // ================== Inventory ==================
    void HandleInventoryToggle()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            inventoryOpen = !inventoryOpen;
            inventoryPanel.SetActive(inventoryOpen);

            Time.timeScale = inventoryOpen ? 0f : 1f;
            Cursor.lockState = inventoryOpen
                ? CursorLockMode.None
                : CursorLockMode.Locked;
            Cursor.visible = inventoryOpen;
        }
    }
}