// SaveManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using UnityEngine;

[Serializable]
public class PlayerStatsData
{
	public float health;
	public float hunger;
	public float thirst;
	public float stamina;
}

[Serializable]
public class PlayerData
{
	public float[] position;
	public float[] rotation;
	public float[] scale;
	public PlayerStatsData stats;
	public int equippedSlot;  // Index of the equipped hotbar slot
}

[Serializable]
public class InventorySaveData
{
	public string itemID;
}

[Serializable]
public class GroundItemSaveData
{
	public string itemID;
	public float[] position;
	public float[] rotation;
	public float[] scale;
}

[Serializable]
public class GameData
{
	public PlayerData player;
	public InventorySaveData[] inventory;
	public GroundItemSaveData[] groundItems;
}

public static class SaveManager
{
	private static string saveDirectory = Path.Combine(Application.dataPath, "SaveData");
	private static string saveFilePath = Path.Combine(saveDirectory, "savefile.json");

	public static void SaveGame(PlayerController player, PlayerStats stats, InventoryManager inventoryManager)
	{
		GameData gameData = new GameData();

		// Save Player Data.
		gameData.player = new PlayerData();
		gameData.player.position = new float[] { player.transform.position.x, player.transform.position.y, player.transform.position.z };
		gameData.player.rotation = new float[] { player.transform.eulerAngles.x, player.transform.eulerAngles.y, player.transform.eulerAngles.z };
		gameData.player.scale = new float[] { player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z };
		gameData.player.stats = new PlayerStatsData()
		{
			health = stats.Health,
			hunger = stats.Hunger,
			thirst = stats.Thirst,
			stamina = stats.Stamina
		};
		// Save the equipped hotbar slot.
		gameData.player.equippedSlot = inventoryManager.selectedHotbarSlot;

		// Save Inventory.
		gameData.inventory = inventoryManager.GetInventorySaveData();

		// Save Ground Items.
		GroundItem[] groundItems = GameObject.FindObjectsOfType<GroundItem>();
		List<GroundItemSaveData> groundSaveList = new List<GroundItemSaveData>();
		foreach (GroundItem gi in groundItems)
		{
			if (gi.item == null) continue;
			GroundItemSaveData saveData = new GroundItemSaveData();
			saveData.itemID = gi.item.ItemID;
			Vector3 pos = gi.transform.position;
			saveData.position = new float[] { pos.x, pos.y, pos.z };
			Vector3 rot = gi.transform.eulerAngles;
			saveData.rotation = new float[] { rot.x, rot.y, rot.z };
			Vector3 scale = gi.transform.localScale;
			saveData.scale = new float[] { scale.x, scale.y, scale.z };
			groundSaveList.Add(saveData);
		}
		gameData.groundItems = groundSaveList.ToArray();

		if (!Directory.Exists(saveDirectory))
			Directory.CreateDirectory(saveDirectory);

		string json = JsonUtility.ToJson(gameData, true);
		File.WriteAllText(saveFilePath, json);
		Debug.Log("Game saved to " + saveFilePath);
	}

	public static void LoadGame(PlayerController player, PlayerStats stats, InventoryManager inventoryManager)
	{
		if (File.Exists(saveFilePath))
		{
			string json = File.ReadAllText(saveFilePath);
			GameData gameData = JsonUtility.FromJson<GameData>(json);
			player.DisableCharacterController();

			// Load Player Data.
			Vector3 pos = new Vector3(gameData.player.position[0], gameData.player.position[1], gameData.player.position[2]);
			player.transform.position = pos;
			Vector3 rot = new Vector3(gameData.player.rotation[0], gameData.player.rotation[1], gameData.player.rotation[2]);
			player.transform.eulerAngles = rot;
			Vector3 scale = new Vector3(gameData.player.scale[0], gameData.player.scale[1], gameData.player.scale[2]);
			player.transform.localScale = scale;
			player.ResetMovement();
			if (stats != null && gameData.player.stats != null)
			{
				stats.Health = gameData.player.stats.health;
				stats.Hunger = gameData.player.stats.hunger;
				stats.Thirst = gameData.player.stats.thirst;
				stats.Stamina = gameData.player.stats.stamina;
			}

			// Load Inventory.
			inventoryManager.LoadInventoryFromSaveData(gameData.inventory);
			// **Set selectedHotbarSlot to -1 to ensure no item is selected upon loading.**
			inventoryManager.SetSelectedHotbarSlot(-1);

			// Remove existing ground items.
			GroundItem[] existingGroundItems = GameObject.FindObjectsOfType<GroundItem>();
			foreach (GroundItem gi in existingGroundItems)
				GameObject.Destroy(gi.gameObject);

			// Load Ground Items.
			foreach (GroundItemSaveData giData in gameData.groundItems)
			{
				InventoryItemData itemData = ItemDatabase.GetItemByID(giData.itemID);
				if (itemData != null && itemData.itemPrefab != null)
				{
					Vector3 groundPos = new Vector3(giData.position[0], giData.position[1], giData.position[2]);
					Vector3 groundRot = new Vector3(giData.rotation[0], giData.rotation[1], giData.rotation[2]);
					Vector3 groundScale = new Vector3(giData.scale[0], giData.scale[1], giData.scale[2]);
					var obj = GameObject.Instantiate(itemData.itemPrefab, groundPos, Quaternion.Euler(groundRot));
					obj.transform.localScale = groundScale;

					

					// Ensure GroundItem script is linked
					GroundItem groundItem = obj.GetComponent<GroundItem>();
					if (groundItem == null)
					{
						groundItem = obj.AddComponent<GroundItem>();
					}
					groundItem.item = itemData;
				}
				else
				{
					Debug.LogWarning("Item not found for ID: " + giData.itemID);
				}
			}

			// **Ensure no item is selected upon loading by removing held items.**
			player.RemoveHeldItems();

			player.EnableCharacterController();
			Debug.Log("Game loaded from " + saveFilePath);
		}
		else
		{
			Debug.LogWarning("Save file not found at " + saveFilePath);
		}
	}
}
