using System;

public class Inventory
{
	private InventoryItemData[] items;
	private int totalSize;
	private int hotbarSize;

	public event Action OnInventoryChanged;

	public Inventory(int totalSize, int hotbarSize)
	{
		this.totalSize = totalSize;
		this.hotbarSize = hotbarSize;
		items = new InventoryItemData[totalSize];
	}

	public bool TryAddItem(InventoryItemData newItem)
	{
		// Fill hotbar indices first
		for (int i = 0; i < hotbarSize; i++)
		{
			if (items[i] == null)
			{
				items[i] = newItem;
				OnInventoryChanged?.Invoke();
				return true;
			}
		}
		for (int i = hotbarSize; i < totalSize; i++)
		{
			if (items[i] == null)
			{
				items[i] = newItem;
				OnInventoryChanged?.Invoke();
				return true;
			}
		}
		return false;
	}

	public InventoryItemData GetItem(int index)
	{
		if (index < 0 || index >= totalSize) return null;
		return items[index];
	}

	public void SetItem(int index, InventoryItemData data)
	{
		if (index < 0 || index >= totalSize) return;
		items[index] = data;
		OnInventoryChanged?.Invoke();
	}

	public void SwapItems(int indexA, int indexB)
	{
		if (indexA < 0 || indexA >= totalSize) return;
		if (indexB < 0 || indexB >= totalSize) return;
		var temp = items[indexA];
		items[indexA] = items[indexB];
		items[indexB] = temp;
		OnInventoryChanged?.Invoke();
	}
	public void RemoveItem(int index)
	{
		if (index < 0 || index >= totalSize) return;
		items[index] = null;
		OnInventoryChanged?.Invoke();
	}

	public int TotalSize => totalSize;
	public int HotbarSize => hotbarSize;
}
