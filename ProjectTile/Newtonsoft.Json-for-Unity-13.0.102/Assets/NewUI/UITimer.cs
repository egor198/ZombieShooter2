using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITimer: MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("����������������� ������� � ����")]
    [SerializeField] private float durationDays = 3f;

    [Header("UI Settings")]
    [Tooltip("UI ����� ��� ����������� ����������� �������")]
    [SerializeField] private TMP_Text timeText;
    [Tooltip("������ ����������� ������� (0 - ���, 1 - ����)")]
    [SerializeField] private string timeFormat = "{0}� {1}�";

    [Header("Target Object")]
    [Tooltip("������ ��� ���������� �� ��������� �������")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject targetObject2;

    // ���� ��� ���������� ������� ������
    private const string StartTimeKey = "TimerStartTime";

    // ���������� �����
    private TimeSpan remainingTime;
    private DateTime endTime;
    private Coroutine timerCoroutine;

    private void Start()
    {
        // ��������� ������� ������������ �������
        if (PlayerPrefs.HasKey(StartTimeKey))
        {
            // �������� ����������� �����
            string savedTime = PlayerPrefs.GetString(StartTimeKey);
            DateTime startTime = DateTime.Parse(savedTime);

            // ������������ �������� �����
            endTime = startTime.AddDays(durationDays);
            remainingTime = endTime - DateTime.Now;

            if (remainingTime <= TimeSpan.Zero)
            {
                // ����� ����� - ��������� ������
                targetObject.SetActive(false);
                targetObject2.SetActive(false);
                UpdateTimeText(TimeSpan.Zero);
            }
            else
            {
                // ����� ��� �� ����� - ��������� ������
                timerCoroutine = StartCoroutine(DeactivationTimer());
                UpdateTimeText(remainingTime);
            }
        }
        else
        {
            // ������ ������ - ��������� ������� �����
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
            // ��������� ���������� �����
            remainingTime = endTime - DateTime.Now;

            // ��������� UI
            UpdateTimeText(remainingTime);

            // ���� 1 �������
            yield return new WaitForSecondsRealtime(1f);
        }

        // ��������� ������ ����� ��������
        targetObject.SetActive(false);
        targetObject2.SetActive(false);
        UpdateTimeText(TimeSpan.Zero);
    }

    // ���������� ������ UI
    private void UpdateTimeText(TimeSpan time)
    {
        if (timeText != null)
        {
            if (time <= TimeSpan.Zero)
            {
                timeText.text = "00� 00�";
            }
            else
            {
                // ������������ ��� � ����
                int days = time.Days;
                int hours = time.Hours;

                // ����������� ������
                timeText.text = string.Format(timeFormat, days, hours);
            }
        }
    }

    // ����� ��� ������ �������
    public void ResetTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // ��������� ����� ����� ������
        PlayerPrefs.SetString(StartTimeKey, DateTime.Now.ToString());
        endTime = DateTime.Now.AddDays(durationDays);
        remainingTime = endTime - DateTime.Now;

        // �������� ������
        targetObject.SetActive(true);
        targetObject2.SetActive(true);

        // ��������� ������
        timerCoroutine = StartCoroutine(DeactivationTimer());
        UpdateTimeText(remainingTime);
    }

    private void OnApplicationQuit()
    {
        // ��������� ����� ��� ������
        PlayerPrefs.Save();
    }

    // ��� ������� � ���������
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.Save();
        }
    }
}
