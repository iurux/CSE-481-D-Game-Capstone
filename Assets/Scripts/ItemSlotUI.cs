using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    public void Set(string itemId, Sprite sprite)
    {
        if (nameText != null)
            nameText.text = itemId;

        if (icon != null)
        {
            icon.gameObject.SetActive(sprite != null);
            if (sprite != null)
                icon.sprite = sprite;
        }
    }
}
