using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class LoadScence : MonoBehaviour
{
    [Header("UI Settings")]
    public Image progressBar; // ����������� � ����������� Image (��� Filled)
    public TextMeshProUGUI progressText; // ��������� ������� ��� ����������� ���������
    public float loadingTime = 3f; // ����� ����� �������� � ��������

    public GameObject panel;

    public Image fon;
    public Sprite[] fonsSprite;

    void Start()
    {
        if (YandexGame.EnvironmentData.deviceType == "mobile")
            fon.sprite = fonsSprite[0];
        else
            fon.sprite = fonsSprite[1];
        // �������� ������� ��������
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float timer = 0f;
        float progress = 0f;

        // �������� ��������
        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            progress = Mathf.Clamp01(timer / loadingTime);

            // ���������� UI
            UpdateProgressUI(progress);

            yield return null;
        }

        panel.SetActive(false);
    }

    void UpdateProgressUI(float progressValue)
    {
        // ���������� progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = progressValue;
        }

        // ���������� ������ ���������
        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progressValue * 100) + "%";
        }
    }

}

