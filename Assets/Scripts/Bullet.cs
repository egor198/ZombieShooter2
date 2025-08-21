using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet: MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;         // �������� �������� ����
    public float destroyDistance = 0.1f; // ��������� ��� �������� �������
    public float verticalOffset = 0.5f;  // �������� ���� ���� �� ������

    private Transform cameraTransform;   // ������ �� ��������� ������
    private bool isMoving = true;        // ���� ��� �������� ��������

    void Start()
    {
        // �������� ������� ������
        cameraTransform = Camera.main.transform;

        // ��������� ������� ������
        if (cameraTransform == null)
        {
            Debug.LogError("Main camera not found! Disabling script.");
            isMoving = false;
        }
    }

    void Update()
    {
        if (!isMoving) return;

        // ������������ ����� ���� ������ (� ������ ������������� ��������)
        Vector3 targetPosition = cameraTransform.position + (Vector3.down * verticalOffset);

        // ������������ ����������� � ��������� �����
        Vector3 direction = (targetPosition - transform.position).normalized;

        // ������� ������ � ����������� ��������� �����
        transform.position += direction * moveSpeed * Time.deltaTime;

        // ��������� ���������� �� ������ (������������ �������)
        float distanceToCamera = Vector3.Distance(transform.position, cameraTransform.position);

        // ���� ������ ���������� ������ � ������ - ����������
        if (distanceToCamera <= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}