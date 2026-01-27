using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerInteract : MonoBehaviour
{
    public GameObject mazeCanvas;
    bool playerInside;

    void Update()
    {
        if (!playerInside) return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenComputer();
        }
    }

    void OpenComputer()
    {
        mazeCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}