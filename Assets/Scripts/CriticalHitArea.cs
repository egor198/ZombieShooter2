using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitArea : MonoBehaviour
{
    public float damageMultiplier = 2f; // ��������� ����� (2 = ������� ����)
    public EnemyController enemyController; // ������ �� ���������� �����

    // ����� ������������� ����� ������������ ���������
    private void Awake()
    {
        if (enemyController == null)
        {
            enemyController = GetComponentInParent<EnemyController>();
        }
    }
}
