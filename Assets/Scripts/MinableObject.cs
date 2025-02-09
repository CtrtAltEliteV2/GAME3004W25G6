using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MineableObject : MonoBehaviour
{
	public int durability = 3;

	public ItemData item;
	public void Mine()
	{
		durability--;
		//Play sound or display particle effect
		Debug.Log($"Mining {gameObject.name}. Durability: {durability}");
		if (durability <= 0)
		{
			Destroy(gameObject);
			Debug.Log($"{gameObject.name} has been mined and destroyed.");
			//Play sound or display particle effect
		}
	}
}

