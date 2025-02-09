using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
	// This script is purely for debug purposes; it will be removed for later builds.
	public Slider healthSlider, staminaSlider, hungerSlider, thirstSlider;
	public Text healthText, staminaText, hungerText, thirstText;
	public GameObject playerState;

	private PlayerStats stats;

	void Awake()
	{
		if (playerState == null)
		{
			Debug.LogError("PlayerState GameObject is not assigned in the Inspector.");
			return;
		}

		stats = playerState.GetComponent<PlayerStats>();

		if (stats == null)
		{
			Debug.LogError("PlayerStats component not found on the PlayerState GameObject.");
			return;
		}

		// Subscribe to the OnStatsChanged event
		stats.OnStatsChanged += UpdateStatsUI;

		// Set max values
		healthSlider.maxValue = stats.Health;
		staminaSlider.maxValue = stats.Stamina;
		hungerSlider.maxValue = stats.Hunger;
		thirstSlider.maxValue = stats.Thirst;

		// Initialize slider values
		UpdateStatsUI();
	}

	void OnDestroy()
	{
		if (stats != null)
		{
			// Unsubscribe from the event to prevent memory leaks
			stats.OnStatsChanged -= UpdateStatsUI;
		}
	}

	void Update()
	{
		if (stats == null) return;

		// Debug key inputs to modify stats
		if (Input.GetKeyDown(KeyCode.N)) stats.Health = Mathf.Max(stats.Health - 10, 0);
		if (Input.GetKeyDown(KeyCode.M)) stats.Stamina = Mathf.Max(stats.Stamina - 10, 0);
		if (Input.GetKeyDown(KeyCode.B)) stats.Hunger = Mathf.Max(stats.Hunger - 10, 0);
		if (Input.GetKeyDown(KeyCode.V)) stats.Thirst = Mathf.Max(stats.Thirst - 10, 0);
		// Note: UpdateStatsUI is no longer called here since it's handled by the event
	}

	void UpdateStatsUI()
	{
		if (stats == null) return;

		healthSlider.value = stats.Health;
		staminaSlider.value = stats.Stamina;
		hungerSlider.value = stats.Hunger;
		thirstSlider.value = stats.Thirst;

		if (healthText != null) healthText.text = $"{stats.Health} / {healthSlider.maxValue}";
		if (staminaText != null) staminaText.text = $"{stats.Stamina} / {staminaSlider.maxValue}";
		if (hungerText != null) hungerText.text = $"{stats.Hunger} / {hungerSlider.maxValue}";
		if (thirstText != null) thirstText.text = $"{stats.Thirst} / {thirstSlider.maxValue}";
	}
}
