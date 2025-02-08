using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	[Header("Player Stats")]
	[SerializeField] private float health = 100f;
	[SerializeField] private float hunger = 100f;
	[SerializeField] private float thirst = 100f;
	[SerializeField] private float stamina = 100f;

	// Define the event using Action delegate
	public event Action OnStatsChanged;

	// Properties with event invocation in setters
	public float Health
	{
		get => health;
		set
		{
			if (Mathf.Approximately(health, value)) return;
			health = value;
			OnStatsChanged?.Invoke();
		}
	}

	public float Hunger
	{
		get => hunger;
		set
		{
			if (Mathf.Approximately(hunger, value)) return;
			hunger = value;
			OnStatsChanged?.Invoke();
		}
	}

	public float Thirst
	{
		get => thirst;
		set
		{
			if (Mathf.Approximately(thirst, value)) return;
			thirst = value;
			OnStatsChanged?.Invoke();
		}
	}

	public float Stamina
	{
		get => stamina;
		set
		{
			if (Mathf.Approximately(stamina, value)) return;
			stamina = value;
			OnStatsChanged?.Invoke();
		}
	}
}
