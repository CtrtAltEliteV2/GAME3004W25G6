using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
	public InventoryManager inventoryManager;
	public int slotIndex;
	public InventoryItem currentItem;
	public Image highLightBorder;

	public void SetSlotItemData(ItemData data)
	{
		if (currentItem != null)
		{
			Destroy(currentItem.gameObject);
			currentItem = null;
		}
		if (data == null) return;

		var itemGO = new GameObject("InventoryItem");
		itemGO.transform.SetParent(transform, false);

		var itemImg = itemGO.AddComponent<Image>();
		itemImg.raycastTarget = true;
		itemImg.preserveAspect = true;

		var cg = itemGO.AddComponent<CanvasGroup>();

		var invItem = itemGO.AddComponent<InventoryItem>();
		invItem.Initialize(data);

		currentItem = invItem;
	}

	public void ClearSlot()
	{
		if (currentItem != null)
		{
			Destroy(currentItem.gameObject);
			currentItem = null;
		}
	}

	public void HighlightSlot(bool h)
	{
		if (highLightBorder != null) highLightBorder.enabled = h;
	}

	public void OnDrop(PointerEventData eventData)
	{
		var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
		if (draggedItem != null)
		{
			var sourceSlot = draggedItem.originalParent?.GetComponent<InventorySlot>();
			if (sourceSlot != null && sourceSlot != this)
			{
				inventoryManager.SwapSlotItems(sourceSlot.slotIndex, slotIndex);
			}
		}
	}

	public void PlaceItem(InventoryItem item)
	{
		var sourceSlot = item.originalParent?.GetComponent<InventorySlot>();
		if (sourceSlot != null && sourceSlot != this)
		{
			inventoryManager.SwapSlotItems(sourceSlot.slotIndex, slotIndex);
		}
	}
}
