using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	// Assign all your InventoryItemData assets in the Inspector.
	public ItemData[] allItems;

	private static Dictionary<string, ItemData> itemLookup;

	private void Awake()
	{
		itemLookup = new Dictionary<string, ItemData>();
		foreach (var item in allItems)
		{
			if (item != null && !itemLookup.ContainsKey(item.ItemID))
				itemLookup.Add(item.ItemID, item);
		}
	}

	public static ItemData GetItemByID(string id)
	{
		if (itemLookup != null && itemLookup.ContainsKey(id))
			return itemLookup[id];
		Debug.LogWarning("Item with ID " + id + " not found in database.");
		return null;
	}
}
