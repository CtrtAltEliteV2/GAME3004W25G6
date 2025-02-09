using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour,
	IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public InventoryItemData itemData;
	public Image itemIcon;
	public CanvasGroup canvasGroup;
	public Transform originalParent;

	void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		itemIcon = GetComponent<Image>();
	}

	public void Initialize(InventoryItemData data)
	{
		itemData = data;
		if (itemIcon != null && data != null)
		{
			itemIcon.sprite = data.itemIcon;
			itemIcon.enabled = (data.itemIcon != null);
			// Make sure preserveAspect is on so it fits the slot better
			itemIcon.preserveAspect = true;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		originalParent = transform.parent;
		transform.SetParent(transform.root);
		transform.SetAsLastSibling();
		if (canvasGroup != null)
			canvasGroup.blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (canvasGroup != null)
			canvasGroup.blocksRaycasts = true;

		// Check if dropped on a slot
		GameObject pointerObj = eventData.pointerCurrentRaycast.gameObject;
		if (pointerObj != null)
		{
			InventorySlot slot = pointerObj.GetComponent<InventorySlot>();
			if (slot != null)
			{
				slot.PlaceItem(this);
				return;
			}
		}

		// Otherwise snap back
		ReturnToOriginalSlot();
	}

	private void ReturnToOriginalSlot()
	{
		transform.SetParent(originalParent, false);
		transform.localPosition = Vector3.zero;
	}
}
