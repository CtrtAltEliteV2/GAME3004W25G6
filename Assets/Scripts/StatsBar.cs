using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    //This script is purely for debug purposes it will be removed for later builds.
    public Slider healthSlider, staminaSlider, hungerSlider, thirstSlider;
    public Text healthText, staminaText, hungerText, thirstText;
    public GameObject playerState;

    private PlayerStats stats;

    void Awake()
    {

        stats = playerState.GetComponent<PlayerStats>();

        // Set max values
        healthSlider.maxValue = stats.Health;
        staminaSlider.maxValue = stats.Stamina;
        hungerSlider.maxValue = stats.Hunger;
        thirstSlider.maxValue = stats.Thirst;

        // Initialize slider values
        UpdateStatsUI();
    }

    void Update()
    {
        if (stats == null) return;

        
        if (Input.GetKeyDown(KeyCode.N)) stats.Health = Mathf.Max(stats.Health - 10, 0);
        if (Input.GetKeyDown(KeyCode.M)) stats.Stamina = Mathf.Max(stats.Stamina - 10, 0);
        if (Input.GetKeyDown(KeyCode.B)) stats.Hunger = Mathf.Max(stats.Hunger - 10, 0);
        if (Input.GetKeyDown(KeyCode.V)) stats.Thirst = Mathf.Max(stats.Thirst - 10, 0);

        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
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
