using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/ConsumableItem")]
public class ConsumableItemData : InventoryItemData
{
	public float healthRestore;
	public float hungerRestore;
	public float thirstRestore;
}
