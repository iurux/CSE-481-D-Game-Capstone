using UnityEngine;
using UnityEngine.InputSystem;

public class MazeInstructionController : MonoBehaviour
{
    public GameObject instructionPanel;
    public GameObject mazePanel;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            instructionPanel.SetActive(false);
            mazePanel.SetActive(true);
        }
    }
}