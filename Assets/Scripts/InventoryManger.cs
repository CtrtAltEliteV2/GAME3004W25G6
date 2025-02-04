using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	[Header("Inventory UI Settings")]
	[SerializeField] private InventoryItemData[] startingItems;

	private InventorySlot[] hotbarSlots;
	private InventorySlot[] extendedInventorySlots;
	private GameObject extendedInventoryPanel;
	private int selectedHotbarSlot = -1;

	public void InitializeInventoryUI()
	{
		// Create Canvas
		GameObject canvasGO = new GameObject("InventoryCanvas");
		Canvas canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;

		CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.matchWidthOrHeight = 0.5f;

		canvasGO.AddComponent<GraphicRaycaster>();

		// Create Hotbar Panel
		GameObject hotbarPanel = new GameObject("HotbarPanel");
		RectTransform hotbarRect = hotbarPanel.AddComponent<RectTransform>();
		hotbarPanel.AddComponent<CanvasRenderer>();
		Image hotbarImage = hotbarPanel.AddComponent<Image>();
		hotbarImage.color = new Color(0, 0, 0, 0.5f);
		hotbarPanel.transform.SetParent(canvasGO.transform, false);

		// Anchor at bottom-center
		hotbarRect.anchorMin = new Vector2(0.5f, 0f);
		hotbarRect.anchorMax = new Vector2(0.5f, 0f);
		hotbarRect.pivot = new Vector2(0.5f, 0f);
		hotbarRect.anchoredPosition = new Vector2(0, 10);
		hotbarRect.sizeDelta = new Vector2(600, 80);

		// HorizontalLayoutGroup so slots appear in a row
		HorizontalLayoutGroup hLayout = hotbarPanel.AddComponent<HorizontalLayoutGroup>();
		hLayout.spacing = 5;
		hLayout.childAlignment = TextAnchor.MiddleCenter;
		// or MiddleLeft if you want them starting from the left
		hLayout.childControlWidth = false;  // don't force slot widths
		hLayout.childControlHeight = false; // don't force slot heights
		hLayout.childForceExpandWidth = false;
		hLayout.childForceExpandHeight = false;
		hLayout.padding = new RectOffset(5, 5, 5, 5);

		// Create 10 hotbar slots
		hotbarSlots = new InventorySlot[10];
		for (int i = 0; i < 10; i++)
		{
			hotbarSlots[i] = CreateSlot("HotbarSlot" + i, hotbarPanel.transform, new Vector2(60, 60));
		}

		// Extended Inventory Panel
		extendedInventoryPanel = new GameObject("ExtendedInventoryPanel");
		RectTransform extRect = extendedInventoryPanel.AddComponent<RectTransform>();
		extendedInventoryPanel.AddComponent<CanvasRenderer>();
		Image extImage = extendedInventoryPanel.AddComponent<Image>();
		extImage.color = new Color(0, 0, 0, 0.8f);
		extendedInventoryPanel.transform.SetParent(canvasGO.transform, false);

		extRect.anchorMin = new Vector2(0.5f, 0.5f);
		extRect.anchorMax = new Vector2(0.5f, 0.5f);
		extRect.pivot = new Vector2(0.5f, 0.5f);
		extRect.anchoredPosition = Vector2.zero;
		extRect.sizeDelta = new Vector2(600, 400);

		GridLayoutGroup grid = extendedInventoryPanel.AddComponent<GridLayoutGroup>();
		grid.cellSize = new Vector2(80, 80);
		grid.spacing = new Vector2(10, 10);
		grid.padding = new RectOffset(10, 10, 10, 10);
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = 6;
		grid.childAlignment = TextAnchor.MiddleCenter;

		// Create 24 extended slots
		extendedInventorySlots = new InventorySlot[24];
		for (int i = 0; i < 24; i++)
		{
			extendedInventorySlots[i] = CreateSlot("ExtendedSlot" + i, extendedInventoryPanel.transform, new Vector2(80, 80));
		}
		extendedInventoryPanel.SetActive(false);

		// If we have starting items, add them
		if (startingItems != null)
		{
			foreach (var itemData in startingItems)
			{
				if (itemData != null)
				{
					AddItem(itemData);
				}
			}
		}
		UpdateHotbarUI();
	}

	private InventorySlot CreateSlot(string name, Transform parent, Vector2 size)
	{
		GameObject slotGO = new GameObject(name);
		RectTransform slotRect = slotGO.AddComponent<RectTransform>();
		slotGO.AddComponent<CanvasRenderer>();
		slotGO.transform.SetParent(parent, false);
		slotRect.sizeDelta = size;

		Image slotBG = slotGO.AddComponent<Image>();
		slotBG.color = Color.gray;
		slotBG.raycastTarget = true;  // must be true for OnDrop events

		InventorySlot slot = slotGO.AddComponent<InventorySlot>();

		// Add a highlight border as a child
		GameObject highlightGO = new GameObject("HighlightBorder");
		RectTransform highlightRect = highlightGO.AddComponent<RectTransform>();
		highlightGO.AddComponent<CanvasRenderer>();
		Image highlightImage = highlightGO.AddComponent<Image>();
		highlightGO.transform.SetParent(slotGO.transform, false);

		highlightRect.anchorMin = Vector2.zero;
		highlightRect.anchorMax = Vector2.one;
		highlightRect.offsetMin = Vector2.zero;
		highlightRect.offsetMax = Vector2.zero;

		// Switch to "Simple" so you can see it easily, and add partial alpha
		highlightImage.type = Image.Type.Simple;
		highlightImage.color = new Color(1f, 1f, 0f, 0.3f);
		highlightImage.raycastTarget = false; // don’t block drops
		highlightImage.enabled = false;

		slot.highLightBorder = highlightImage;

		slot.ClearSlot();
		return slot;
	}

	public void ToggleExtendedInventory()
	{
		if (extendedInventoryPanel)
		{
			extendedInventoryPanel.SetActive(!extendedInventoryPanel.activeSelf);
		}
	}

	public void SetSelectedHotbarSlot(int index)
	{
		if (index >= 0 && index < hotbarSlots.Length)
		{
			selectedHotbarSlot = index;
			UpdateHotbarUI();
		}
	}

	private void UpdateHotbarUI()
	{
		for (int i = 0; i < hotbarSlots.Length; i++)
		{
			hotbarSlots[i].HighlightSlot(i == selectedHotbarSlot);
		}
	}

	public void AddItem(InventoryItemData newItem)
	{
		if (newItem == null)
		{
			Debug.LogWarning("Attempted to add null item.");
			return;
		}

		foreach (var slot in hotbarSlots)
		{
			if (slot.currentItem == null)
			{
				slot.AddItem(newItem);
				UpdateHotbarUI();
				return;
			}
		}
		foreach (var slot in extendedInventorySlots)
		{
			if (slot.currentItem == null)
			{
				slot.AddItem(newItem);
				return;
			}
		}
		Debug.Log("Inventory is full!");
	}

	public InventoryItem GetItemInHotbar(int index)
	{
		if (index < 0 || index >= hotbarSlots.Length) return null;
		return hotbarSlots[index].currentItem;
	}
}
