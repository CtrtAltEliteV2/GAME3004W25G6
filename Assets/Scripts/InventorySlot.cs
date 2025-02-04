using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
	public InventoryItem currentItem;
	public Image highLightBorder;

	public void AddItem(InventoryItemData newItemData)
	{
		if (currentItem != null)
		{
			Debug.LogWarning($"Slot {name} is already occupied!");
			return;
		}

		// Create a new child GameObject for the item
		GameObject itemGO = new GameObject("InventoryItem");
		itemGO.transform.SetParent(transform, false);
		itemGO.transform.localPosition = Vector3.zero;

		// UI Components: an Image for the sprite, plus a CanvasGroup
		Image itemImage = itemGO.AddComponent<Image>();
		itemImage.raycastTarget = true;
		// So it scales within the slot but keeps aspect ratio
		itemImage.preserveAspect = true;

		CanvasGroup cg = itemGO.AddComponent<CanvasGroup>();

		InventoryItem inventoryItem = itemGO.AddComponent<InventoryItem>();
		inventoryItem.Initialize(newItemData);

		currentItem = inventoryItem;

		if (highLightBorder != null)
		{
			highLightBorder.transform.SetAsLastSibling();
		}
	}

	public void ClearSlot()
	{
		if (currentItem != null)
		{
			Destroy(currentItem.gameObject);
			currentItem = null;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		InventoryItem droppedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
		if (droppedItem != null)
		{
			InventorySlot sourceSlot = droppedItem.originalParent?.GetComponent<InventorySlot>();
			if (sourceSlot != null && sourceSlot != this)
			{
				SwapItems(sourceSlot, this);
			}
		}
	}

	public void PlaceItem(InventoryItem item)
	{
		InventorySlot sourceSlot = item.originalParent?.GetComponent<InventorySlot>();
		if (sourceSlot != null && sourceSlot != this)
		{
			SwapItems(sourceSlot, this);
		}
	}

	private void SwapItems(InventorySlot source, InventorySlot target)
	{
		InventoryItem sourceItem = source.currentItem;
		InventoryItem targetItem = target.currentItem;

		source.currentItem = targetItem;
		if (targetItem != null)
		{
			targetItem.transform.SetParent(source.transform, false);
			targetItem.transform.localPosition = Vector3.zero;
			targetItem.originalParent = source.transform;
		}

		target.currentItem = sourceItem;
		sourceItem.transform.SetParent(target.transform, false);
		sourceItem.transform.localPosition = Vector3.zero;
		sourceItem.originalParent = target.transform;
	}

	public void HighlightSlot(bool highlight)
	{
		if (highLightBorder != null)
		{
			highLightBorder.enabled = highlight;
		}
	}
}
