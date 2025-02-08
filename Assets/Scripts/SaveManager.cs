using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public PlayerStatsData stats;
    public InventoryItemData[] inventory;

}
public static class SaveManager 
{
	private static string saveDirectory = Path.Combine(Application.dataPath, "SaveData");
	private static string saveFilePath = Path.Combine(saveDirectory, "savefile.json");

	public static void SaveGame(PlayerController player, PlayerStats stats, InventoryItemData[] inventory)
	{
		PlayerData playerData = new PlayerData();
		playerData.position = new float[] { player.transform.position.x, player.transform.position.y, player.transform.position.z };
		playerData.rotation = new float[] { player.transform.eulerAngles.x, player.transform.eulerAngles.y, player.transform.eulerAngles.z };
		playerData.stats = new PlayerStatsData()
		{
			health = stats.Health,
			hunger = stats.Hunger,
			thirst = stats.Thirst,
			stamina = stats.Stamina
		};
		playerData.inventory = inventory;

		// Ensure the directory exists
		if (!Directory.Exists(saveDirectory))
		{
			Directory.CreateDirectory(saveDirectory);
		}

		string json = JsonUtility.ToJson(playerData, true);
		File.WriteAllText(saveFilePath, json);

		Debug.Log("Game saved to " + saveFilePath);
	}
	public static void LoadGame(PlayerController player, PlayerStats stats, InventoryManager inventoryManager)
	{
		if (File.Exists(saveFilePath))
		{
			string json = File.ReadAllText(saveFilePath);
			PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
			player.DisableCharacterController();
			// Load position
			Vector3 position = new Vector3(
				playerData.position[0],
				playerData.position[1],
				playerData.position[2]
			);
			player.transform.position = position;

			// Load rotation
			Vector3 rotation = new Vector3(
				playerData.rotation[0],
				playerData.rotation[1],
				playerData.rotation[2]
			);
			player.transform.eulerAngles = rotation;
			
			player.ResetMovement();
			if (stats != null && playerData.stats != null)
			{
				stats.Health = playerData.stats.health;
				stats.Hunger = playerData.stats.hunger;
				stats.Thirst = playerData.stats.thirst;
				stats.Stamina = playerData.stats.stamina;
			}

			// Reset movement variables
			player.EnableCharacterController();

			Debug.Log("Game loaded from " + saveFilePath);
		}
		else
		{
			Debug.LogWarning("Save file not found at " + saveFilePath);
		}
	}



}
