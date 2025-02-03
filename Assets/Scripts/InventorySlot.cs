using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
	public Image icon;
	public Image highLightBorder;
	public InventoryItem item;

	public void AddItem(InventoryItem newItem)
	{
		if(newItem == null)
		{
			Debug.LogWarning("Invalid item.");
			return;
		}
		item = newItem;
		icon.sprite = item.itemIcon;
		icon.enabled = true;
	}
	public void ClearSlot()
	{
		item = null;
		icon.sprite = null;
		icon.enabled = false;
	}
	public void HighlightSlot(bool highlight)
	{
		if(highLightBorder == null)
		{
			Debug.LogWarning("No highlight border found.");
			return;
		}
		highLightBorder.enabled = highlight;
	}
}
