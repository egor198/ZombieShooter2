using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Activation Settings")]
    public bool activationFlag = false;
    public bool resetFlag = false;

    [Header("Waypoints")]
    public List<Waypoint> waypoints = new List<Waypoint>();

    [Header("Enemy References")]
    public List<EnemyController> enemies = new List<EnemyController>();

    private GameManager gameManager;

    private readonly object waypointLock = new object();

    private void Update()
    {
        if (activationFlag)
        {
            ActivateAllEnemies();
            activationFlag = false;
        }

        if (resetFlag)
        {
            ResetAllEnemies();
            resetFlag = false;
        }
    }

    public void ActivateAllEnemies()
    {
        ResetWaypoints();

        // ������� null-������ ����� ����������
        enemies.RemoveAll(e => e == null);

        if (gameManager.Difficult == GameManager.difficult.hardcore)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.isAlive)
                {
                    enemy.ActivateEnemy();
                }
            }
        }
        else
        {
            // ������� ��������� ������ ����� ������
            List<EnemyController> aliveEnemies = new List<EnemyController>();
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.isAlive)
                {
                    aliveEnemies.Add(enemy);
                }
            }

            // ���������� ������ ��������
            int halfCount = Mathf.CeilToInt(aliveEnemies.Count / 2f);
            for (int i = 0; i < halfCount; i++)
            {
                aliveEnemies[i].ActivateEnemy();
            }

            // ������� ���������� (������ ��������)
            for (int i = halfCount; i < aliveEnemies.Count; i++)
            {
                RemoveEnemy(aliveEnemies[i]);
                aliveEnemies[i].gameObject.SetActive(false);
            }
        }
    }


    public void ResetAllEnemies()
    {
        // ������� ������������ ������
        enemies.RemoveAll(e => e == null);

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetEnemy();
            }
        }

        ResetWaypoints();
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        if (enemy == null) return;

        // ����������� �����
        if (enemy.targetWaypoint != null)
        {
            enemy.targetWaypoint.isOccupied = false;
            enemy.targetWaypoint.occupiedBy = null;
        }

        // ������� �� ������
        enemies.Remove(enemy);

        // ���������� ������
        //Destroy(enemy.gameObject);

        // ���� �������������� ������ �� ��������
        if (enemies.Count == 0)
        {
            StartCoroutine(delayed());
        }
    }

    public IEnumerator delayed()
    {
        yield return new WaitForSeconds(2f);
        gameManager.NextPoint();
    }

    public void ActDeactEnemy(bool act)
    {
        foreach(var enemy in enemies) 
        {
            enemy.gameObject.SetActive(act);
        }
    }

    public Waypoint GetRandomFreeWaypoint()
    {
        lock (waypointLock) // ���������� ��� ������������������
        {
            List<Waypoint> freeWaypoints = new List<Waypoint>();

            foreach (var waypoint in waypoints)
            {
                if (!waypoint.isOccupied)
                {
                    freeWaypoints.Add(waypoint);
                }
            }

            if (freeWaypoints.Count == 0) return null;

            // �������� ��������� �����
            int randomIndex = Random.Range(0, freeWaypoints.Count);
            return freeWaypoints[randomIndex];
        }
    }

    private void ResetWaypoints()
    {
        foreach (var waypoint in waypoints)
        {
            waypoint.ResetPosition();
        }
    }

    void Start()
    {
        FindAllWaypoints();
        FindAllEnemies();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void FindAllWaypoints()
    {
        waypoints.Clear();
        // ���� ������ � �������� ��������
        waypoints.AddRange(GetComponentsInChildren<Waypoint>());
    }

    public void FindAllEnemies()
    {
        enemies.Clear();
        // ���� ������ � �������� ��������
        enemies.AddRange(GetComponentsInChildren<EnemyController>());
    }

    // ��������� ������ � ��������� ��� ��������
#if UNITY_EDITOR
    [ContextMenu("Find All Children")]
    public void FindAllChildrenInEditor()
    {
        FindAllWaypoints();
        FindAllEnemies();
        Debug.Log($"Found {enemies.Count} enemies and {waypoints.Count} waypoints in children");
    }
#endif

    public void AddEnemy(EnemyController enemy)
    {
        if (enemy == null) return;

        // ��������� ����� � ������
        enemies.Add(enemy);

        // ��������� �������� ��� ����������� ��������
        enemy.transform.SetParent(transform);

        // ���������� �����, ���� �������� �������
        if (activationFlag || isActiveAndEnabled)
        {
            enemy.ActivateEnemy();
        }
        else
        {
            enemy.gameObject.SetActive(false);
        }
    }

    public void AddSpawnPoint(GameObject spawnPoint)
    {
        // ��������� ����� ������ ��� �������� ������
        spawnPoint.transform.SetParent(transform);
    }
}
