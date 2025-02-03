using UnityEngine;

public class GroundItem : MonoBehaviour
{
	// Reference to the InventoryItem ScriptableObject representing this item.
	public InventoryItem item;

	private void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag("Player"))
		{
			InventoryManager inventoryUI = other.GetComponent<InventoryManager>();
			if (inventoryUI != null && item != null)
			{
				// Add the item to the extended inventory (or hotbar, as you prefer).
				inventoryUI.AddItem(item);
				Destroy(gameObject);
			}
		}
	}
}
