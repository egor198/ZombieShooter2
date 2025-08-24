using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITimer: MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Продолжительность таймера в днях")]
    [SerializeField] private float durationDays = 3f;

    [Header("UI Settings")]
    [Tooltip("UI текст для отображения оставшегося времени")]
    [SerializeField] private TMP_Text timeText;
    [Tooltip("Формат отображения времени (0 - дни, 1 - часы)")]
    [SerializeField] private string timeFormat = "{0}д {1}ч";

    [Header("Target Object")]
    [Tooltip("Объект для отключения по истечении времени")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject targetObject2;

    // Ключ для сохранения времени старта
    private const string StartTimeKey = "TimerStartTime";

    // Оставшееся время
    private TimeSpan remainingTime;
    private DateTime endTime;
    private Coroutine timerCoroutine;

    private void Start()
    {
        // Проверяем наличие сохраненного времени
        if (PlayerPrefs.HasKey(StartTimeKey))
        {
            // Получаем сохраненное время
            string savedTime = PlayerPrefs.GetString(StartTimeKey);
            DateTime startTime = DateTime.Parse(savedTime);

            // Рассчитываем конечное время
            endTime = startTime.AddDays(durationDays);
            remainingTime = endTime - DateTime.Now;

            if (remainingTime <= TimeSpan.Zero)
            {
                // Время вышло - отключаем объект
                targetObject.SetActive(false);
                targetObject2.SetActive(false);
                UpdateTimeText(TimeSpan.Zero);
            }
            else
            {
                // Время еще не вышло - запускаем отсчет
                timerCoroutine = StartCoroutine(DeactivationTimer());
                UpdateTimeText(remainingTime);
            }
        }
        else
        {
            // Первый запуск - сохраняем текущее время
            endTime = DateTime.Now.AddDays(durationDays);
            PlayerPrefs.SetString(StartTimeKey, DateTime.Now.ToString());
            remainingTime = endTime - DateTime.Now;

            timerCoroutine = StartCoroutine(DeactivationTimer());
            UpdateTimeText(remainingTime);
        }
    }

    private IEnumerator DeactivationTimer()
    {
        while (remainingTime > TimeSpan.Zero)
        {
            // Обновляем оставшееся время
            remainingTime = endTime - DateTime.Now;

            // Обновляем UI
            UpdateTimeText(remainingTime);

            // Ждем 1 секунду
            yield return new WaitForSecondsRealtime(1f);
        }

        // Отключаем объект после ожидания
        targetObject.SetActive(false);
        targetObject2.SetActive(false);
        UpdateTimeText(TimeSpan.Zero);
    }

    // Обновление текста UI
    private void UpdateTimeText(TimeSpan time)
    {
        if (timeText != null)
        {
            if (time <= TimeSpan.Zero)
            {
                timeText.text = "00д 00ч";
            }
            else
            {
                // Рассчитываем дни и часы
                int days = time.Days;
                int hours = time.Hours;

                // Форматируем строку
                timeText.text = string.Format(timeFormat, days, hours);
            }
        }
    }

    // Метод для сброса таймера
    public void ResetTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Сохраняем новое время старта
        PlayerPrefs.SetString(StartTimeKey, DateTime.Now.ToString());
        endTime = DateTime.Now.AddDays(durationDays);
        remainingTime = endTime - DateTime.Now;

        // Включаем объект
        targetObject.SetActive(true);
        targetObject2.SetActive(true);

        // Запускаем таймер
        timerCoroutine = StartCoroutine(DeactivationTimer());
        UpdateTimeText(remainingTime);
    }

    private void OnApplicationQuit()
    {
        // Сохраняем время при выходе
        PlayerPrefs.Save();
    }

    // Для отладки в редакторе
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.Save();
        }
    }
}
