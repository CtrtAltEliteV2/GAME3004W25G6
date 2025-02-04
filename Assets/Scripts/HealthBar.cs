using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthCounter;
    public GameObject playerState;

    private PlayerStats stats;

    void Awake()
    {
        stats = playerState.GetComponent<PlayerStats>();
        slider.maxValue = stats.Health;
        slider.value = stats.Health;
        UpdateHealthText();
    }

    void Update()
    {
        if (stats == null || slider == null) return;

        // Reduce health when pressing "N"
        if (Input.GetKeyDown(KeyCode.N))
        {
            stats.Health = Mathf.Max(stats.Health - 10, 0);
            slider.value = stats.Health;
            UpdateHealthText();
        }
    }

    void UpdateHealthText()
    {
        if (healthCounter != null)
        {
            healthCounter.text = $"{stats.Health} / {slider.maxValue}";
        }
    }
}
