using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAdsCoin: MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Время появления элемента (секунды)")]
    public float fadeDuration = 1f;

    [Tooltip("Задержка между элементами (секунды)")]
    public float delayBetweenElements = 1f;

    [Header("Ссылки на UI элементы")]
    public CanvasGroup[] uiElements;

    private Coroutine animationCoroutine;

    void OnEnable()
    {
        // Сброс всех элементов в невидимое состояние
        foreach (var element in uiElements)
        {
            element.alpha = 0f;
            element.interactable = false;
            element.blocksRaycasts = false;
        }

        // Запускаем последовательность
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(ShowElementsSequence());
    }

    void OnDisable()
    {
        // Останавливаем анимацию при выключении объекта
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    IEnumerator ShowElementsSequence()
    {
        foreach (var element in uiElements)
        {
            // Запускаем анимацию для текущего элемента
            yield return StartCoroutine(FadeInElement(element));

            // Задержка перед следующим элементом
            yield return new WaitForSeconds(delayBetweenElements);
        }
    }

    IEnumerator FadeInElement(CanvasGroup element)
    {
        float elapsedTime = 0f;

        // Анимация прозрачности
        while (elapsedTime < fadeDuration)
        {
            element.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Финализируем значения
        element.alpha = 1f;

        // Активируем взаимодействие ТОЛЬКО ПОСЛЕ завершения анимации
        element.interactable = true;
        element.blocksRaycasts = true;
    }
}
