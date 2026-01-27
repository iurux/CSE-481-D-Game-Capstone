using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MazeHintUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ruleHintText;

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ruleHintText.text =
                "Rule 1 Hint:\n" +
                "Rule 1 is normal.\n" +
                "Movement works as expected.";
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            ruleHintText.text =
                "Rule 2 Hint:\n" +
                "You can ONLY move on purple blocks.\n" +
                "Normal tiles are blocked.";
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            ruleHintText.text =
                "Rule 3 Hint:\n" +
                "All movement directions are reversed.\n" +
                "Try moving opposite of your intent.";
        }
    }
}