using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	[Header("Inventory UI Settings")]
	[Tooltip("Optional starting items for the inventory.")]
	[SerializeField] private InventoryItem[] startingItems;

	private InventorySlot[] hotbarSlots;
	private InventorySlot[] extendedInventorySlots;
	private GameObject extendedInventoryPanel;
	private int selectedHotbarSlot = -1;

	public void InitializeInventoryUI()
	{
		// Create Canvas
		GameObject canvasGO = new GameObject("Canvas");
		Canvas canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasGO.AddComponent<CanvasScaler>();
		canvasGO.AddComponent<GraphicRaycaster>();

		// Create Hotbar Panel
		GameObject hotbarPanel = new GameObject("HotbarPanel");
		RectTransform hotbarRect = hotbarPanel.AddComponent<RectTransform>();
		hotbarPanel.AddComponent<CanvasRenderer>();
		Image hotbarImage = hotbarPanel.AddComponent<Image>();
		hotbarImage.color = new Color(0, 0, 0, 0.5f);
		hotbarPanel.transform.SetParent(canvasGO.transform);

		hotbarRect.anchorMin = new Vector2(0.5f, 0);
		hotbarRect.anchorMax = new Vector2(0.5f, 0);
		hotbarRect.pivot = new Vector2(0.5f, 0);
		hotbarRect.anchoredPosition = new Vector2(0, 10);
		hotbarRect.sizeDelta = new Vector2(400, 80);

		HorizontalLayoutGroup hLayout = hotbarPanel.AddComponent<HorizontalLayoutGroup>();
		hLayout.spacing = 5;
		hLayout.childAlignment = TextAnchor.MiddleCenter;

		// Create hotbar slots (10 slots)
		int hotbarCount = 10;
		hotbarSlots = new InventorySlot[hotbarCount];
		for (int i = 0; i < hotbarCount; i++)
		{
			hotbarSlots[i] = CreateSlot("HotbarSlot" + i, hotbarPanel.transform, new Vector2(60, 60));
		}

		// Add starting items
		if (startingItems != null)
		{
			foreach (var item in startingItems)
			{
				if (item != null)
					AddItem(item);
			}
		}
		UpdateHotbarUI();

		// Create Extended Inventory Panel
		extendedInventoryPanel = new GameObject("ExtendedInventoryPanel");
		RectTransform extRect = extendedInventoryPanel.AddComponent<RectTransform>();
		extendedInventoryPanel.AddComponent<CanvasRenderer>();
		Image extImage = extendedInventoryPanel.AddComponent<Image>();
		extImage.color = new Color(0, 0, 0, 0.8f);
		extendedInventoryPanel.transform.SetParent(canvasGO.transform);

		extRect.anchorMin = new Vector2(0.5f, 0.5f);
		extRect.anchorMax = new Vector2(0.5f, 0.5f);
		extRect.pivot = new Vector2(0.5f, 0.5f);
		extRect.anchoredPosition = Vector2.zero;
		extRect.sizeDelta = new Vector2(600, 400);

		GridLayoutGroup grid = extendedInventoryPanel.AddComponent<GridLayoutGroup>();
		grid.cellSize = new Vector2(60, 60);
		grid.spacing = new Vector2(10, 10);
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = 6;

		// Create extended inventory slots (24 slots)
		int extCount = 24;
		extendedInventorySlots = new InventorySlot[extCount];
		for (int i = 0; i < extCount; i++)
		{
			extendedInventorySlots[i] = CreateSlot("ExtendedSlot" + i, extendedInventoryPanel.transform, new Vector2(60, 60));
		}
		extendedInventoryPanel.SetActive(false);
	}

	private InventorySlot CreateSlot(string name, Transform parent, Vector2 size)
	{
		GameObject slotGO = new GameObject(name);
		RectTransform slotRect = slotGO.AddComponent<RectTransform>();
		slotGO.AddComponent<CanvasRenderer>();
		slotGO.transform.SetParent(parent);
		slotRect.sizeDelta = size;

		Image slotImage = slotGO.AddComponent<Image>();
		slotImage.color = Color.gray;

		InventorySlot slot = slotGO.AddComponent<InventorySlot>();

		// Icon
		GameObject iconGO = new GameObject("Icon");
		RectTransform iconRect = iconGO.AddComponent<RectTransform>();
		iconGO.AddComponent<CanvasRenderer>();
		Image iconImage = iconGO.AddComponent<Image>();
		iconGO.transform.SetParent(slotGO.transform);
		iconRect.anchorMin = Vector2.zero;
		iconRect.anchorMax = Vector2.one;
		iconRect.offsetMin = Vector2.zero;
		iconRect.offsetMax = Vector2.zero;
		iconImage.enabled = false;

		// Highlight Border
		GameObject highlightGO = new GameObject("HighlightBorder");
		RectTransform highlightRect = highlightGO.AddComponent<RectTransform>();
		highlightGO.AddComponent<CanvasRenderer>();
		Image highlightImage = highlightGO.AddComponent<Image>();
		highlightGO.transform.SetParent(slotGO.transform);
		highlightRect.anchorMin = Vector2.zero;
		highlightRect.anchorMax = Vector2.one;
		highlightRect.offsetMin = Vector2.zero;
		highlightRect.offsetMax = Vector2.zero;
		highlightImage.color = Color.yellow;
		highlightImage.type = Image.Type.Sliced;
		highlightImage.enabled = false;

		slot.icon = iconImage;
		slot.highLightBorder = highlightImage;
		slot.ClearSlot();
		return slot;
	}

	public void SetSelectedHotbarSlot(int index)
	{
		if (index >= 0 && index < hotbarSlots.Length)
		{
			selectedHotbarSlot = index;
			UpdateHotbarUI();
		}
	}

	public void UpdateHotbarUI()
	{
		for (int i = 0; i < hotbarSlots.Length; i++)
			hotbarSlots[i].HighlightSlot(i == selectedHotbarSlot);
	}

	public void ToggleExtendedInventory()
	{
		if (extendedInventoryPanel)
			extendedInventoryPanel.SetActive(!extendedInventoryPanel.activeSelf);
	}

	public void AddItem(InventoryItem newItem)
	{
		if (newItem == null)
		{
			Debug.LogWarning("Attempted to add null item.");
			return;
		}

		// Try hotbar first
		foreach (var slot in hotbarSlots)
		{
			if (slot.item == null)
			{
				slot.AddItem(newItem);
				UpdateHotbarUI();
				return;
			}
		}

		// Then extended inventory
		foreach (var slot in extendedInventorySlots)
		{
			if (slot.item == null)
			{
				slot.AddItem(newItem);
				return;
			}
		}
		Debug.Log("Inventory is full!");
	}

	public InventoryItem GetItemInHotbar(int index)
	{
		return hotbarSlots[index].item;
	}
}
