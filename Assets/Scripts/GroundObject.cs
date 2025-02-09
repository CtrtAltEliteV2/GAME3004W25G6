using System;
using UnityEngine;

public class GroundObject : MonoBehaviour
{
	public ItemData item;

	private void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag("Player"))
		{
			InventoryManager inventoryUI = other.GetComponent<InventoryManager>();
			if (inventoryUI != null && item != null)
			{
				inventoryUI.AddItem(item);
				Destroy(gameObject);
			}
		}
	}
}
