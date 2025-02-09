using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/ConsumableItem")]
public class ConsumableItemData : ItemData
{
	public float healthRestore;
	public float staminaRestore;
	public float hungerRestore;
	public float thirstRestore;
}
