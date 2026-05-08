using UnityEngine;
using TMPro;
using System;

public class MissionTimer : MonoBehaviour
{
    public event Action<float> OnTimeRatioChanged;

    [Header("UI Reference")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private UnityEngine.UI.Image relojBackground;

    [Header("Settings")]
    [SerializeField] private float missionDuration = 100f;
    [SerializeField] private float blinkSpeed = 4f;

    private float currentTime;
    private bool isRunning = false;
    private float lastNotifiedRatio = -1f;

    public float CurrentTime => currentTime;
    Color redColor = new Color(1f, 180f/255f, 171f/255f, 1f);

    void Start()
    {
        ResetTimer();
        StartTimer();
    }

    public void ResetTimer()
    {
        currentTime = missionDuration;
        UpdateUI();
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    void Update()
    {
        if (!isRunning) return;

        if (currentTime > 0)
        {
            // Si en tu consola sale dt=0, asegúrate de que el TimeScale en Unity sea 1
            currentTime -= Time.deltaTime;
            if (currentTime < 0) currentTime = 0;

            UpdateUI();
            NotifyRatio();
        }
        else
        {
            isRunning = false;
            NotifyRatio(); // Notificar el 0 final
            Debug.Log("¡Tiempo agotado! Fallaste la misión.");
        }
    }

    private void NotifyRatio()
    {
        float ratio = Mathf.Clamp01(currentTime / missionDuration);
        // Solo notificamos si hay un cambio real para optimizar rendimiento
        if (!Mathf.Approximately(ratio, lastNotifiedRatio))
        {
            lastNotifiedRatio = ratio;
            OnTimeRatioChanged?.Invoke(ratio);
        }
    }

    private void UpdateUI()
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Feedback visual en el reloj cuando queda poco tiempo (15 seg)
        if (currentTime <= 15f && currentTime > 0)
        {
            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            timeText.color = Color.Lerp(Color.white, redColor, blink);
        }
        else if (currentTime <= 0)
        {
            timeText.color = redColor;
        }
    }
}