using System.Collections.Generic;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    // use item ID to save：ex "student card"
    private HashSet<string> items = new HashSet<string>();

    public bool Has(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return items.Contains(itemId.ToLower().Trim());
    }

    public void Add(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        items.Add(itemId.ToLower().Trim());
        Debug.Log("[Inventory] Added: " + itemId);
    }
}
