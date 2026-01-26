using System.Collections.Generic;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    public InventoryUI ui; // 不拖也行，会自动找

    private HashSet<string> items = new HashSet<string>();

    private void Start()
    {
    }

    public bool Has(string itemId)
    {
        return !string.IsNullOrEmpty(itemId) && items.Contains(itemId);
    }

    public void Add(string itemId, Sprite icon = null)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        // 已有就不重复加
        if (!items.Add(itemId)) return;

        if (ui != null)
            ui.AddItem(itemId, icon);

        Debug.Log("[Inventory] Add: " + itemId);

    }
}
