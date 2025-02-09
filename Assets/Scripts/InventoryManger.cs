using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	[SerializeField] private InventoryItemData[] startingItems;

	private InventorySlot[] hotbarSlots;
	private InventorySlot[] extendedSlots;
	private GameObject extendedPanel;
	public int selectedHotbarSlot = -1;

	public Inventory inventory;
	private const int HOTBAR_COUNT = 10;
	private const int EXTENDED_COUNT = 24;

	public void InitializeInventoryUI()
	{
		inventory = new Inventory(HOTBAR_COUNT + EXTENDED_COUNT, HOTBAR_COUNT);
		inventory.OnInventoryChanged += RefreshUI;

		var canvasGO = new GameObject("InventoryCanvas");
		var canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		var scaler = canvasGO.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.matchWidthOrHeight = 0.5f;
		canvasGO.AddComponent<GraphicRaycaster>();

		var hotbarPanel = new GameObject("HotbarPanel");
		var hotbarRect = hotbarPanel.AddComponent<RectTransform>();
		hotbarPanel.transform.SetParent(canvasGO.transform, false);
		hotbarRect.anchorMin = new Vector2(0.5f, 0f);
		hotbarRect.anchorMax = new Vector2(0.5f, 0f);
		hotbarRect.pivot = new Vector2(0.5f, 0f);
		hotbarRect.anchoredPosition = new Vector2(0, 10);
		hotbarRect.sizeDelta = new Vector2(600, 80);

		var hLayout = hotbarPanel.AddComponent<HorizontalLayoutGroup>();
		hLayout.spacing = 5;
		hLayout.childAlignment = TextAnchor.MiddleCenter;
		hLayout.childControlWidth = false;
		hLayout.childControlHeight = false;
		hLayout.childForceExpandWidth = false;
		hLayout.childForceExpandHeight = false;
		hLayout.padding = new RectOffset(5, 5, 5, 5);

		hotbarSlots = new InventorySlot[HOTBAR_COUNT];
		for (int i = 0; i < HOTBAR_COUNT; i++)
		{
			hotbarSlots[i] = CreateSlot("HotbarSlot" + i, hotbarPanel.transform, new Vector2(60, 60), i);
		}

		extendedPanel = new GameObject("ExtendedInventoryPanel");
		var extRect = extendedPanel.AddComponent<RectTransform>();
		extendedPanel.AddComponent<CanvasRenderer>();
		var extImage = extendedPanel.AddComponent<Image>();
		extImage.color = new Color(0, 0, 0, 0.8f);
		extendedPanel.transform.SetParent(canvasGO.transform, false);
		extRect.anchorMin = new Vector2(0.5f, 0.5f);
		extRect.anchorMax = new Vector2(0.5f, 0.5f);
		extRect.pivot = new Vector2(0.5f, 0.5f);
		extRect.anchoredPosition = Vector2.zero;
		extRect.sizeDelta = new Vector2(600, 400);

		var grid = extendedPanel.AddComponent<GridLayoutGroup>();
		grid.cellSize = new Vector2(80, 80);
		grid.spacing = new Vector2(10, 10);
		grid.padding = new RectOffset(10, 10, 10, 10);
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = 6;
		grid.childAlignment = TextAnchor.MiddleCenter;

		extendedSlots = new InventorySlot[EXTENDED_COUNT];
		for (int i = 0; i < EXTENDED_COUNT; i++)
		{
			int index = HOTBAR_COUNT + i;
			extendedSlots[i] = CreateSlot("ExtendedSlot" + i, extendedPanel.transform, new Vector2(80, 80), index);
		}
		extendedPanel.SetActive(false);

		if (startingItems != null)
		{
			foreach (var itemData in startingItems)
			{
				if (itemData != null) inventory.TryAddItem(itemData);
			}
		}
		RefreshUI();
		UpdateHotbarUI();
	}

	private InventorySlot CreateSlot(string name, Transform parent, Vector2 size, int index)
	{
		var slotGO = new GameObject(name);
		var slotRect = slotGO.AddComponent<RectTransform>();
		slotGO.transform.SetParent(parent, false);
		slotRect.sizeDelta = size;
		var slotBG = slotGO.AddComponent<Image>();
		slotBG.color = Color.gray;
		slotBG.raycastTarget = true;

		var slot = slotGO.AddComponent<InventorySlot>();
		slot.inventoryManager = this;
		slot.slotIndex = index;

		var highlightGO = new GameObject("HighlightBorder");
		var highlightRect = highlightGO.AddComponent<RectTransform>();
		highlightGO.AddComponent<CanvasRenderer>();
		var highlightImg = highlightGO.AddComponent<Image>();
		highlightGO.transform.SetParent(slotGO.transform, false);
		highlightRect.anchorMin = Vector2.zero;
		highlightRect.anchorMax = Vector2.one;
		highlightRect.offsetMin = Vector2.zero;
		highlightRect.offsetMax = Vector2.zero;
		highlightImg.type = Image.Type.Simple;
		highlightImg.color = new Color(1f, 1f, 0f, 0.3f);
		highlightImg.raycastTarget = false;
		highlightImg.enabled = false;
		slot.highLightBorder = highlightImg;

		slot.ClearSlot();
		return slot;
	}

	public void RefreshUI()
	{
		for (int i = 0; i < HOTBAR_COUNT; i++)
		{
			var data = inventory.GetItem(i);
			hotbarSlots[i].SetSlotItemData(data);
		}
		for (int i = 0; i < EXTENDED_COUNT; i++)
		{
			int index = HOTBAR_COUNT + i;
			var data = inventory.GetItem(index);
			extendedSlots[i].SetSlotItemData(data);
		}
	}

	public void ToggleExtendedInventory()
	{
		if (extendedPanel != null) extendedPanel.SetActive(!extendedPanel.activeSelf);
	}

	// Modified Method to Allow Deselecting
	public void SetSelectedHotbarSlot(int i)
	{
		if (i < -1 || i >= HOTBAR_COUNT) return; // Allow -1 for deselection
		selectedHotbarSlot = i;
		UpdateHotbarUI();
	}

	public void UpdateHotbarUI()
	{
		for (int i = 0; i < HOTBAR_COUNT; i++)
		{
			hotbarSlots[i].HighlightSlot(i == selectedHotbarSlot);
		}
	}

	public void AddItem(InventoryItemData newItem)
	{
		if (newItem == null) return;
		bool added = inventory.TryAddItem(newItem);
		if (!added) Debug.Log("Inventory full!");
	}

	public InventoryItem GetItemInHotbar(int i)
	{
		if (i < 0 || i >= HOTBAR_COUNT) return null;
		return hotbarSlots[i].currentItem;
	}
	public InventoryItem GetCurrentItem()
	{
		return GetItemInHotbar(selectedHotbarSlot);
	}

	public void SwapSlotItems(int indexA, int indexB)
	{
		inventory.SwapItems(indexA, indexB);
	}
	public InventoryItemData[] GetInventoryItemData()
	{
		return inventory.GetItemData();
	}
	public void ClearAllSlots()
	{
		for (int i = 0; i < inventory.TotalSize; i++)
		{
			inventory.SetItem(i, null);
		}
	}
	public InventorySaveData[] GetInventorySaveData()
	{
		InventoryItemData[] items = inventory.GetItemData();
		InventorySaveData[] saveData = new InventorySaveData[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null)
				saveData[i] = new InventorySaveData() { itemID = items[i].ItemID };
			else
				saveData[i] = null;
		}
		return saveData;
	}

	public void LoadInventoryFromSaveData(InventorySaveData[] saveData)
	{
		ClearAllSlots();
		for (int i = 0; i < saveData.Length; i++)
		{
			if (saveData[i] != null)
			{
				string id = saveData[i].itemID;
				InventoryItemData itemData = ItemDatabase.GetItemByID(id);
				if (itemData != null)
					inventory.SetItem(i, itemData);
			}
		}
		RefreshUI();
	}
}
