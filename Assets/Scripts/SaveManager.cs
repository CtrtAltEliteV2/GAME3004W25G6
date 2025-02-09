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
public class GroundObjectSaveData
{
	public string itemID;
	public float[] position;
	public float[] rotation;
	public float[] scale;
}
[Serializable]
public class MineableObjectSaveData
{
	public string itemID;
	public float[] position;
	public float[] rotation;
	public float[] scale;
	public int durability;
}

[Serializable]
public class GameData
{
	public PlayerData player;
	public InventorySaveData[] inventory;
	public GroundObjectSaveData[] groundItems;
	public MineableObjectSaveData[] mineableObjects;
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
		GroundObject[] groundItems = GameObject.FindObjectsOfType<GroundObject>();
		List<GroundObjectSaveData> groundSaveList = new List<GroundObjectSaveData>();
		foreach (GroundObject gi in groundItems)
		{
			if (gi.item == null) continue;
			GroundObjectSaveData saveData = new GroundObjectSaveData();
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

		MineableObject[] mineableObjects = GameObject.FindObjectsOfType<MineableObject>();
		List<MineableObjectSaveData> mineableSaveList = new List<MineableObjectSaveData>();
		foreach (MineableObject mo in mineableObjects)
		{
			if (mo.item == null) continue;
			MineableObjectSaveData saveData = new MineableObjectSaveData();
			saveData.itemID = mo.item.ItemID;
			Vector3 pos = mo.transform.position;
			saveData.position = new float[] { pos.x, pos.y, pos.z };
			Vector3 rot = mo.transform.eulerAngles;
			saveData.rotation = new float[] { rot.x, rot.y, rot.z };
			Vector3 scale = mo.transform.localScale;
			saveData.scale = new float[] { scale.x, scale.y, scale.z };
			saveData.durability = mo.durability;
			
			mineableSaveList.Add(saveData);
		}
		gameData.mineableObjects = mineableSaveList.ToArray();
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
			GroundObject[] existingGroundItems = GameObject.FindObjectsOfType<GroundObject>();
			foreach (GroundObject gi in existingGroundItems)
				GameObject.Destroy(gi.gameObject);

			// Load Ground Items.
			foreach (GroundObjectSaveData giData in gameData.groundItems)
			{
				ItemData itemData = ItemDatabase.GetItemByID(giData.itemID);
				if (itemData != null && itemData.itemPrefab != null)
				{
					Vector3 groundPos = new Vector3(giData.position[0], giData.position[1], giData.position[2]);
					Vector3 groundRot = new Vector3(giData.rotation[0], giData.rotation[1], giData.rotation[2]);
					Vector3 groundScale = new Vector3(giData.scale[0], giData.scale[1], giData.scale[2]);
					var obj = GameObject.Instantiate(itemData.itemPrefab, groundPos, Quaternion.Euler(groundRot));
					obj.transform.localScale = groundScale;

					

					// Ensure GroundItem script is linked
					GroundObject groundItem = obj.GetComponent<GroundObject>();
					if (groundItem == null)
					{
						groundItem = obj.AddComponent<GroundObject>();
					}
					groundItem.item = itemData;
				}
				else
				{
					Debug.LogWarning("Item not found for ID: " + giData.itemID);
				}
			}

			// Remove existing mineable objects.
			MineableObject[] existingMineableObjects = GameObject.FindObjectsOfType<MineableObject>();
			foreach (MineableObject mo in existingMineableObjects)
				GameObject.Destroy(mo.gameObject);

			// Load Mineable Objects.
			foreach (MineableObjectSaveData moData in gameData.mineableObjects)
			{
				ItemData itemData = ItemDatabase.GetItemByID(moData.itemID);
				if (itemData != null && itemData.itemPrefab != null)
				{
					Vector3 mineablePos = new Vector3(moData.position[0], moData.position[1], moData.position[2]);
					Vector3 mineableRot = new Vector3(moData.rotation[0], moData.rotation[1], moData.rotation[2]);
					Vector3 mineableScale = new Vector3(moData.scale[0], moData.scale[1], moData.scale[2]);
					var obj = GameObject.Instantiate(itemData.itemPrefab, mineablePos, Quaternion.Euler(mineableRot));
					obj.transform.localScale = mineableScale;
					// Ensure MineableObject script is linked
					MineableObject mineableObject = obj.GetComponent<MineableObject>();
					if (mineableObject == null)
					{
						mineableObject = obj.AddComponent<MineableObject>();
					}
					mineableObject.item = itemData;
					mineableObject.durability = moData.durability;
				}
				else
				{
					Debug.LogWarning("Item not found for ID: " + moData.itemID);
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
