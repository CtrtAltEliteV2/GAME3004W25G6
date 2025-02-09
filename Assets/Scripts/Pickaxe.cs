// Pickaxe.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : MonoBehaviour, IUsable
{
	[SerializeField] private float miningRange = 5f;
	private Camera playerCamera;

	void Start()
	{

		// Retrieve the player's camera from the parent hierarchy
		playerCamera = GetComponentInParent<Camera>();
		if (playerCamera == null)
		{
			Debug.LogError("Player's Camera component not found in parent hierarchy.");
		}
	}

	public void Use()
	{
		Debug.Log("Mining with Pickaxe...");

		if (playerCamera == null)
		{
			Debug.LogError("Player camera not found. Mining action cannot be performed.");
			return;
		}

		// Calculate the center point of the screen
		Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		Ray ray = playerCamera.ScreenPointToRay(screenCenter);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, miningRange))
		{
			MineableObject mineable = hit.collider.GetComponent<MineableObject>();
			if (mineable != null)
			{
				mineable.Mine();

				
			}
			else
			{
				Debug.Log("Hit object is not mineable.");
			}
		}
		else
		{
			Debug.Log("No mineable object in range.");
		}
	}

}
