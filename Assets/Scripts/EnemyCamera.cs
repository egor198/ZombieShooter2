using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCamera : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 5f;
    private Transform showPosition;
    private Transform hidePosition;

    private float timer;

    private bool isAnimating = false;
    private Coroutine currentAnimation;

    void Start()
    {
        // Начальная позиция (скрыта под камерой)
        showPosition = Camera.main.transform.GetChild(1);
        hidePosition = Camera.main.transform.GetChild(0);
        transform.rotation = hidePosition.transform.rotation;
        transform.position = hidePosition.position;
        ShowObject();
    }

    public void ShowObject()
    {
        if (isAnimating) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateObject(showPosition));
    }

    public void HideObject()
    {
        if (isAnimating) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateObject(hidePosition));
        GameManager.isCameraEnemy = false;
        Destroy(gameObject, 2);
    }

    private IEnumerator AnimateObject(Transform targetPosition)
    {
        isAnimating = true;
        Vector3 startPosition = transform.position;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * animationSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, progress);
            yield return null;
        }

        transform.position = targetPosition.position;
        isAnimating = false;
    }
}
