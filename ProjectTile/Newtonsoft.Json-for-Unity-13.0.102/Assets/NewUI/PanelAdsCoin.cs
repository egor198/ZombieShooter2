using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAdsCoin: MonoBehaviour
{
    [Header("���������")]
    [Tooltip("����� ��������� �������� (�������)")]
    public float fadeDuration = 1f;

    [Tooltip("�������� ����� ���������� (�������)")]
    public float delayBetweenElements = 1f;

    [Header("������ �� UI ��������")]
    public CanvasGroup[] uiElements;

    private Coroutine animationCoroutine;

    void OnEnable()
    {
        // ����� ���� ��������� � ��������� ���������
        foreach (var element in uiElements)
        {
            element.alpha = 0f;
            element.interactable = false;
            element.blocksRaycasts = false;
        }

        // ��������� ������������������
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(ShowElementsSequence());
    }

    void OnDisable()
    {
        // ������������� �������� ��� ���������� �������
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
            // ��������� �������� ��� �������� ��������
            yield return StartCoroutine(FadeInElement(element));

            // �������� ����� ��������� ���������
            yield return new WaitForSeconds(delayBetweenElements);
        }
    }

    IEnumerator FadeInElement(CanvasGroup element)
    {
        float elapsedTime = 0f;

        // �������� ������������
        while (elapsedTime < fadeDuration)
        {
            element.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ������������ ��������
        element.alpha = 1f;

        // ���������� �������������� ������ ����� ���������� ��������
        element.interactable = true;
        element.blocksRaycasts = true;
    }
}
