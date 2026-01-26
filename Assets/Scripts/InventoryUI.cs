using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemContainer;     // 拖 ItemContainer
    public ItemSlotUI itemSlotPrefab;   // 拖 ItemSlotPrefab
    public Sprite defaultIcon;          // 可选

    private Dictionary<string, ItemSlotUI> slots = new Dictionary<string, ItemSlotUI>();

    public void AddItem(string itemId, Sprite icon)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        // 已经有就不再加
        if (slots.ContainsKey(itemId)) return;

        ItemSlotUI slot = Instantiate(itemSlotPrefab, itemContainer);
        slot.name = "Slot_" + itemId;

        slot.Set(itemId, icon != null ? icon : defaultIcon);
        slots[itemId] = slot;
    }
}
