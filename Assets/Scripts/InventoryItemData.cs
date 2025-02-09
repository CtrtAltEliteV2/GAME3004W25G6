using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItemData : ScriptableObject
{
	[Header("Identification")]
	[SerializeField, HideInInspector] private string itemID; // Hidden in Inspector

	[Header("Display Data")]
	public string itemName;
	public string itemDescription;
	public Sprite itemIcon;

	[Header("Prefab")]
	public GameObject itemPrefab;

	public string ItemID => itemID;

#if UNITY_EDITOR
	private void OnValidate()
	{
		// Auto-generate an ID if one isn’t assigned
		if (string.IsNullOrEmpty(itemID))
		{
			itemID = Guid.NewGuid().ToString();
			// Mark the asset dirty so the change is saved:
			UnityEditor.EditorUtility.SetDirty(this);
		}
	}
#endif
}
