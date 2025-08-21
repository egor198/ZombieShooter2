using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSpawnPoint: MonoBehaviour
{
    public SpawnPoint[] points;

    void Start()
    {
        // ������������� �������� ��� �������� SpawnPoint
        RefreshChildPoints();
    }

    // ����� ��� ���������� ������ �������� �����
    public void RefreshChildPoints()
    {
        // �������� ��� ���������� SpawnPoint � �������� ��������
        points = GetComponentsInChildren<SpawnPoint>();

        // ���� ����� ��������� ������ ���� (���� ������ ���� �� SpawnPoint)
        // points = GetComponentsInChildren<SpawnPoint>().Where(p => p.transform != transform).ToArray();
    }

    public bool isDestroyd()
    {
        // ��������� ������ �����
        foreach (SpawnPoint point in points)
        {
            // ���� ����� ���������� ��� null, ����������
            if (point == null) continue;

            // ���� ���� �� ���� ����� �� ���������� - ���������� false
            if (!point.isDestroyed)
            {
                return false;
            }
        }

        // ��� ����� ����������
        return true;
    }

    // ��������-������ ��� ���������� ������ � ����������
#if UNITY_EDITOR
    [ContextMenu("Refresh Child Points")]
    private void RefreshInEditor()
    {
        RefreshChildPoints();
        Debug.Log($"Found {points.Length} child spawn points");
    }
#endif
}
