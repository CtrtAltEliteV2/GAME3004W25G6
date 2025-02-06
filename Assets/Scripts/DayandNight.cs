using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
{
    //variable to store a light source
    [SerializeField] private Light sun;

    //variable to store the time of the day
    [SerializeField, Range(0, 24)] private float timeOfDay;

    //variable to store the speed of rotation
    [SerializeField] private float sunRotationSpeed;

    //variables to store the lighting presets
    [Header("LightingPreset")]
    [SerializeField] private Gradient skyColor;

    [SerializeField] private Gradient equatorColor;
    [SerializeField] private Gradient sunColor;
    //function to update Sun's rotation

    private void Update()
    {
        timeOfDay += Time.deltaTime * sunRotationSpeed;
        if (timeOfDay > 24)
            timeOfDay = 0;
        UpdateSunRotation();
        UpdateLighting();
    }

    private void OnValidate()
    {
        UpdateSunRotation();
        UpdateLighting();
    }

    private void UpdateSunRotation()
    {
        float sunRotation = Mathf.Lerp(-90, 270, timeOfDay / 24);
        sun.transform.rotation = Quaternion.Euler(sunRotation, sun.transform.rotation.y, sun.transform.rotation.z);
    }

    //fuction to update the lighting

    private void UpdateLighting()
    {
        float timeFraction = timeOfDay / 24;
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        sun.color = sunColor.Evaluate(timeFraction);
    }
}