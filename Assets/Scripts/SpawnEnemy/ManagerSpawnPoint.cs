using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSpawnPoint: MonoBehaviour
{
    public SpawnPoint[] points;

    void Start()
    {
        // Автоматически собираем все дочерние SpawnPoint
        RefreshChildPoints();
    }

    // Метод для обновления списка дочерних точек
    public void RefreshChildPoints()
    {
        // Получаем все компоненты SpawnPoint у дочерних объектов
        points = GetComponentsInChildren<SpawnPoint>();

        // Если нужно исключить самого себя (если скрипт тоже на SpawnPoint)
        // points = GetComponentsInChildren<SpawnPoint>().Where(p => p.transform != transform).ToArray();
    }

    public bool isDestroyd()
    {
        // Проверяем каждую точку
        foreach (SpawnPoint point in points)
        {
            // Если точка уничтожена или null, пропускаем
            if (point == null) continue;

            // Если хотя бы одна точка не уничтожена - возвращаем false
            if (!point.isDestroyed)
            {
                return false;
            }
        }

        // Все точки уничтожены
        return true;
    }

    // Редактор-хелпер для обновления списка в инспекторе
#if UNITY_EDITOR
    [ContextMenu("Refresh Child Points")]
    private void RefreshInEditor()
    {
        RefreshChildPoints();
        Debug.Log($"Found {points.Length} child spawn points");
    }
#endif
}
