using UnityEngine;

public class ComputerInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject mazeCanvas;

    /// <summary>
    /// Called by FPSControllerSimple when player presses E on this object
    /// </summary>
    public void Interact()
    {
        OpenComputer();
    }

    void OpenComputer()
    {
        Debug.Log("OpenComputer called");

        if (mazeCanvas != null)
        {
            mazeCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("MazeCanvas reference is missing!");
        }

        // Pause game world (optional)
        Time.timeScale = 0f;

        // Unlock mouse for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}