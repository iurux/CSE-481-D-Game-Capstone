using UnityEngine;

public class CardReaderInteractable : MonoBehaviour
{
    [Header("Unlock State")]
    public bool accessGranted = false;

    public CircuitPuzzleManager circuitPuzzle;

    [Header("Dialogue")]
    [TextArea]
    public string[] powerOffDialogue =
    {
        "There is no power",
        "I need to turn on the power first."
    };

    [TextArea]
    public string[] mazeLockedDialogue =
    {
        "I don't have access.",
        "I need to grant access to my student ID first.",
        "I think I can use the computer over there."
    };

    [TextArea]
    public string[] grantAccessDialogue =
    {
        "Access granted."
    };

    [TextArea]
    public string[] alreadyGrantedDialogue =
    {
        "Access is already granted."
    };


    public void TryInteract(DialogueUI dialogueUI)
    {
        // 电源没启动：读卡器锁住
        if (circuitPuzzle != null && !circuitPuzzle.IsSolved)
        {
            dialogueUI?.StartDialogue(powerOffDialogue);
            return;
        }

        // 迷宫没解：读卡器锁住，播你原来的 mazeLockedDialogue
        if (!MazeProgress.mazeSolved)
        {
            dialogueUI?.StartDialogue(mazeLockedDialogue);
            return;
        }

        // 迷宫解了：第一次刷卡 -> grant
        if (!accessGranted)
        {
            accessGranted = true;
            dialogueUI?.StartDialogue(grantAccessDialogue);
            return;
        }

        // 已经 grant：可选提示
        dialogueUI?.StartDialogue(alreadyGrantedDialogue);
    }
}
