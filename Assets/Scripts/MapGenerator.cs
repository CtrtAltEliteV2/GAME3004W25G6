using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[Header("Map Size")]
	[SerializeField] private Vector2 mapSize = new Vector2(100f, 100f);

	[Header("Ground Items")]
	[SerializeField] private int numberOfItems = 10;
	[SerializeField] private List<ItemData> itemsToSpawn;

	private void Start()
	{
		GenerateMap();
	}

	private void GenerateMap()
	{
		for (int i = 0; i < numberOfItems; i++)
		{
			Vector3 position = new Vector3(
				Random.Range(-mapSize.x / 2f, mapSize.x / 2f),
				0.5f, 
				Random.Range(-mapSize.y / 2f, mapSize.y / 2f)
			);

			// Select a random ItemData from the list
			ItemData randomItemData = itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

			if (randomItemData != null && randomItemData.itemPrefab != null)
			{
				GameObject itemObject = Instantiate(randomItemData.itemPrefab, position, Quaternion.identity);

				GroundObject groundObject = itemObject.GetComponent<GroundObject>();
				if (groundObject == null)
				{
					groundObject = itemObject.AddComponent<GroundObject>();
				}
				groundObject.item = randomItemData;
			}
			else
			{
				Debug.LogWarning("ItemData is null or missing prefab.");
			}
		}
	}
}
