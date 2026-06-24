/*
 * Arquitectura: HUD/UI
 * Script: MissionTimer
 * Rol: Presenta informacion de tiempo y emite senales simples para otros sistemas.
 * Modulo: Gestiona el texto visual del temporizador de mision.
 * Relaciones: Puede avisar expiracion con OnTimerExpired, pero no consume vida ni decide muerte.
 * Importante: el dano debe venir de fuentes de dano como DamageOverTimeSource; HealthBehaviour recibe ese dano y maneja muerte/notificacion.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;
using System;

public class MissionTimer : MonoBehaviour
{
    public event Action<float> OnTimeRatioChanged;
    public event Action OnTimerExpired;

    [Header("UI Reference")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private UnityEngine.UI.Image relojBackground;

    [Header("Settings")]
    [SerializeField] private float missionDuration = 100f;
    [SerializeField] private float blinkSpeed = 4f;

    private float currentTime;
    private bool isRunning;
    private float lastNotifiedRatio = -1f;
    private bool expirationNotified;
    private bool restoredFromSave;

    public float CurrentTime => currentTime;
    public float MissionDuration => missionDuration;
    public bool IsRunning => isRunning;
    public bool IsExpired => currentTime <= 0f;

    private readonly Color redColor = new Color(1f, 180f / 255f, 171f / 255f, 1f);

    private void Start()
    {
        if (restoredFromSave)
            return;

        ResetTimer();
        StartTimer();
    }

    public void ResetTimer()
    {
        currentTime = missionDuration;
        expirationNotified = false;
        UpdateUI();
        NotifyRatio();
    }

    public void RestoreTime(float timeRemaining)
    {
        // SaveLoad hidrata el temporizador; MissionTimer sigue siendo presentacion/tiempo.
        currentTime = Mathf.Clamp(timeRemaining, 0f, missionDuration);
        expirationNotified = currentTime <= 0f;
        isRunning = currentTime > 0f;
        restoredFromSave = true;
        UpdateUI();
        NotifyRatio();
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    private void Update()
    {
        if (!isRunning)
            return;

        if (currentTime > 0f)
        {
            // Si en consola deltaTime aparece en 0, revisar que Time.timeScale sea 1.
            currentTime = Mathf.Max(currentTime - Time.deltaTime, 0f);

            UpdateUI();
            NotifyRatio();

            if (currentTime <= 0f)
            {
                isRunning = false;
                NotifyExpired();
                Debug.Log("Tiempo agotado. MissionTimer aviso expiracion.");
            }
        }
    }

    private void NotifyRatio()
    {
        float ratio = missionDuration <= 0f ? 0f : Mathf.Clamp01(currentTime / missionDuration);

        if (!Mathf.Approximately(ratio, lastNotifiedRatio))
        {
            lastNotifiedRatio = ratio;
            OnTimeRatioChanged?.Invoke(ratio);
        }
    }

    private void NotifyExpired()
    {
        if (expirationNotified)
            return;

        expirationNotified = true;
        OnTimerExpired?.Invoke();
    }

    private void UpdateUI()
    {
        if (timeText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 15f && currentTime > 0f)
        {
            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            timeText.color = Color.Lerp(Color.white, redColor, blink);
        }
        else if (currentTime <= 0f)
        {
            timeText.color = redColor;
        }
        else
        {
            timeText.color = Color.white;
        }
    }
}
