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
    public Image progressBar; // Изображение с компонентом Image (тип Filled)
    public TextMeshProUGUI progressText; // Текстовый элемент для отображения процентов
    public float loadingTime = 3f; // Общее время загрузки в секундах

    public GameObject panel;

    public Image fon;
    public Sprite[] fonsSprite;

    void Start()
    {
        if (YandexGame.EnvironmentData.deviceType == "mobile")
            fon.sprite = fonsSprite[0];
        else
            fon.sprite = fonsSprite[1];
        // Начинаем процесс загрузки
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float timer = 0f;
        float progress = 0f;

        // Имитация загрузки
        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            progress = Mathf.Clamp01(timer / loadingTime);

            // Обновление UI
            UpdateProgressUI(progress);

            yield return null;
        }

        panel.SetActive(false);
    }

    void UpdateProgressUI(float progressValue)
    {
        // Обновление progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = progressValue;
        }

        // Обновление текста процентов
        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progressValue * 100) + "%";
        }
    }

}

