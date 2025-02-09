using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStats : MonoBehaviour
{
	[Header("Player Stats")]
	[SerializeField] private float health = 100f;
	[SerializeField] private float hunger = 100f;
	[SerializeField] private float thirst = 100f;
	[SerializeField] private float stamina = 100f;

	public float Health { get => health; set => health = value; }
	public float Hunger { get => hunger; set => hunger = value; }
	public float Thirst { get => thirst; set => thirst = value; }
	public float Stamina { get => stamina; set => stamina = value; }

}

