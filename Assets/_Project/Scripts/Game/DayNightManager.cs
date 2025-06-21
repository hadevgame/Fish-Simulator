using UnityEngine;
using System;

public class DayNightManager : MonoBehaviour
{
    [Header("Skyboxes")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material eveningSkybox;
    [SerializeField] private Material nightSkybox;

    [Header("Light Control")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float dayIntensity = 1.2f;
    [SerializeField] private float eveningIntensity = 1f;
    [SerializeField] private float nightIntensity = 0.5f;

    [Header("Thời điểm chuyển phase (giờ trong game)")]
    [SerializeField] private int hourDayStart = 9;
    [SerializeField] private int hourEveningStart = 15;
    [SerializeField] private int hourNightStart = 19;

    private void Update()
    {
        if (GameTimeManager.Instance != null)
        {
            UpdateLighting(GameTimeManager.Instance.GetGameTime());
        }
    }

    public void UpdateLighting(DateTime gameTime)
    {
        int hour = gameTime.Hour;
        int minute = gameTime.Minute;
        float time = hour + (minute / 60f);

        Material currentSkybox = daySkybox;
        float targetIntensity = dayIntensity;

        if (time >= hourNightStart || time < hourDayStart)
        {
            currentSkybox = nightSkybox;
            targetIntensity = nightIntensity;
        }
        else if (time >= hourEveningStart)
        {
            // Transition from evening to night
            float t = Mathf.InverseLerp(hourEveningStart, hourNightStart, time);
            RenderSettings.skybox = eveningSkybox;
            targetIntensity = Mathf.Lerp(eveningIntensity, nightIntensity, t);
            directionalLight.intensity = targetIntensity;
            return;
        }
        else if (time >= hourDayStart && time < hourEveningStart)
        {
            // Transition from day to evening
            float t = Mathf.InverseLerp(hourDayStart, hourEveningStart, time);
            RenderSettings.skybox = daySkybox;
            targetIntensity = Mathf.Lerp(dayIntensity, eveningIntensity, t);
            directionalLight.intensity = targetIntensity;
            return;
        }

        RenderSettings.skybox = currentSkybox;
        directionalLight.intensity = targetIntensity;
    }

}

