using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MineableObject.cs
using UnityEngine;

public class MineableObject : MonoBehaviour
{
	public int durability = 3;

	public void Mine()
	{
		durability--;
		Debug.Log($"Mining {gameObject.name}. Durability: {durability}");
		if (durability <= 0)
		{
			Destroy(gameObject);
			Debug.Log($"{gameObject.name} has been mined and destroyed.");
		}
	}
}

