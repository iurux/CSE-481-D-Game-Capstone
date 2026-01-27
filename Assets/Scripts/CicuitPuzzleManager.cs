using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CircuitPuzzleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject puzzleCanvasPanel; // The entire UI panel for the minigame
    public Button closeButton;           // Optional: Button to exit the puzzle

    [Header("System References")]
    public PowerSystem powerSystem;      // Reference to the 3D lighting system

    [Header("Status")]
    public bool IsSolved { get; private set; } = false;

    private List<PipeRotator> allPipes = new List<PipeRotator>();

    private void Start()
    {
        // Find all pipe scripts under this object
        PipeRotator[] pipes = GetComponentsInChildren<PipeRotator>();
        foreach (var pipe in pipes)
        {
            allPipes.Add(pipe);
            pipe.RandomizeRotation(); // Scramble the puzzle on start
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);

        // Ensure puzzle is hidden at game start
        puzzleCanvasPanel.SetActive(false);
    }

    // Call this method from your Player Interaction script
    public void OpenPuzzle()
    {
        if (IsSolved) return; // Do not reopen if already completed

        puzzleCanvasPanel.SetActive(true);
        
        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePuzzle()
    {
        puzzleCanvasPanel.SetActive(false);

        // Lock cursor back to FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckWinCondition()
    {
        foreach (var pipe in allPipes)
        {
            if (!pipe.IsCorrect()) return; // If even one pipe is wrong, stop checking
        }

        // If loop finishes, all pipes are correct
        OnPuzzleSolved();
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved! Power Restoring...");
        IsSolved = true;
        
        // Disable interaction on all pipes so they can't be rotated anymore
        foreach (var pipe in allPipes)
        {
            // If your PipeRotator has a way to disable clicking, do it here
            pipe.enabled = false; 
        }

        // Wait 0.5 seconds so the player sees the connected lines, then finish
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (powerSystem != null) 
        {
            powerSystem.RestorePower();
        }
        
        // Find the player controller and call the resume function we created
        FPSControllerSimple player = FindObjectOfType<FPSControllerSimple>();
        if (player != null)
        {
            player.ClosePuzzleAndResumeGame();
        }
        else
        {
            // Fallback if player script is not found
            ClosePuzzle();
        }
    }
}