using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItemData : ScriptableObject
{
	public string itemName;
	public string itemDescription;
	public Sprite itemIcon;
	public GameObject itemPrefab;
}
