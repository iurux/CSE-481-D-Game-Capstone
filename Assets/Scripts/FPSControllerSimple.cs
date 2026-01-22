using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSControllerSimple : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 4.5f;
    public float lookSensitivity = 0.12f; // 鼠标灵敏度
    public float gravity = -20f;

    float pitch;
    Vector3 vel;
    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- Mouse Look (Input System) ---
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
            mouseDelta = Mouse.current.delta.ReadValue();

        float mx = mouseDelta.x * lookSensitivity;
        float my = mouseDelta.y * lookSensitivity;

        transform.Rotate(0, mx, 0);
        pitch = Mathf.Clamp(pitch - my, -80f, 80f);
        cam.localEulerAngles = new Vector3(pitch, 0, 0);

        // --- WASD Move (Input System) ---
        Vector2 move2 = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) move2.y += 1;
            if (Keyboard.current.sKey.isPressed) move2.y -= 1;
            if (Keyboard.current.dKey.isPressed) move2.x += 1;
            if (Keyboard.current.aKey.isPressed) move2.x -= 1;
        }
        move2 = Vector2.ClampMagnitude(move2, 1f);

        Vector3 move = transform.right * move2.x + transform.forward * move2.y;
        cc.Move(move * moveSpeed * Time.deltaTime);

        // --- Gravity ---
        if (cc.isGrounded && vel.y < 0) vel.y = -1f;
        vel.y += gravity * Time.deltaTime;
        cc.Move(vel * Time.deltaTime);

        // --- Cursor toggle ---
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }
    }
}
