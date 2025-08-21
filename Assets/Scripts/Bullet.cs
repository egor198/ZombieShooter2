using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet: MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;         // Скорость движения пули
    public float destroyDistance = 0.1f; // Дистанция для удаления объекта
    public float verticalOffset = 0.5f;  // Смещение цели вниз от камеры

    private Transform cameraTransform;   // Ссылка на трансформ камеры
    private bool isMoving = true;        // Флаг для контроля движения

    void Start()
    {
        // Получаем главную камеру
        cameraTransform = Camera.main.transform;

        // Проверяем наличие камеры
        if (cameraTransform == null)
        {
            Debug.LogError("Main camera not found! Disabling script.");
            isMoving = false;
        }
    }

    void Update()
    {
        if (!isMoving) return;

        // Рассчитываем точку ниже камеры (с учетом вертикального смещения)
        Vector3 targetPosition = cameraTransform.position + (Vector3.down * verticalOffset);

        // Рассчитываем направление к смещенной точке
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Двигаем объект в направлении смещенной точки
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Проверяем расстояние до камеры (оригинальной позиции)
        float distanceToCamera = Vector3.Distance(transform.position, cameraTransform.position);

        // Если объект достаточно близко к камере - уничтожаем
        if (distanceToCamera <= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}