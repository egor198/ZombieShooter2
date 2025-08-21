using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitArea : MonoBehaviour
{
    public float damageMultiplier = 2f; // Множитель урона (2 = двойной урон)
    public EnemyController enemyController; // Ссылка на контроллер врага

    // Можно автоматически найти родительский компонент
    private void Awake()
    {
        if (enemyController == null)
        {
            enemyController = GetComponentInParent<EnemyController>();
        }
    }
}
