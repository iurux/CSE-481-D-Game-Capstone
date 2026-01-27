using UnityEngine;
using UnityEngine.EventSystems;

public class PipeRotator : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    // The Z-axis angles that are considered "correct" for this specific pipe.
    // Example: A straight pipe might be correct at 0 and 180.
    public float[] correctRotations; 
    
    [Header("Debug")]
    [SerializeField] private bool isPlacedCorrectly = false;

    private RectTransform rectTransform;
    private CircuitPuzzleManager puzzleManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        puzzleManager = GetComponentInParent<CircuitPuzzleManager>();
    }

    // Called by the Manager when the game starts to scramble the puzzle
    public void RandomizeRotation()
    {
        // Randomly choose 0, 90, 180, or 270 degrees
        float randomZ = Random.Range(0, 4) * 90f;
        rectTransform.localEulerAngles = new Vector3(0, 0, randomZ);
        
        CheckCorrection();
    }

    // Detects mouse click on the UI element
    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager.IsSolved) return; // Prevent interaction if game is already won

        // Rotate 90 degrees clockwise
        rectTransform.Rotate(0, 0, -90f);
        
        CheckCorrection();
        
        // Notify the manager to check if the whole puzzle is solved
        puzzleManager.CheckWinCondition(); 
    }

    private void CheckCorrection()
    {
        // Normalize angle to 0-360 range
        float currentZ = rectTransform.localEulerAngles.z;
        currentZ = Mathf.Round(currentZ); 
        
        if (currentZ >= 360) currentZ -= 360;
        if (currentZ < 0) currentZ += 360;

        isPlacedCorrectly = false;

        // Check if current angle matches any of the correct answers
        foreach (float target in correctRotations)
        {
            if (Mathf.Abs(currentZ - target) < 1f) // Allow small floating point error
            {
                isPlacedCorrectly = true;
                break;
            }
        }
    }

    public bool IsCorrect()
    {
        return isPlacedCorrectly;
    }
}