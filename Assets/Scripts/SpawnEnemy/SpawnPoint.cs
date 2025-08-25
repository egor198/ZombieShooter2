using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int maxEnemies = 5;
    public float spawnInterval = 3f;
    public GameObject[] enemyPrefab;
    public float patrolRadius = 5f; // Радиус патрулирования
    public bool isStart;

    [Header("Patrol Center Offset")]
    public Vector3 patrolCenterOffset = Vector3.zero; // Новый параметр: смещение центра

    [Header("Health Settings")]
    public float maxHealth = 200;
    public float currentHealth;

    public List<SpawnerEnemyController> spawnedEnemies = new List<SpawnerEnemyController>();
    private float nextSpawnTime;
    public bool isDestroyed;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.red; // Цвет подсветки
    [Range(0, 10)] public float highlightIntensity = 2f; // Интенсивность подсветки

    private Renderer rend;
    private Color originalColor;
    private bool isHighlighted = false;
    private GameManager gameManager;

    private TextMeshProUGUI textHp;
    private Image sliderHp;
    public bool isNonDestroyed;

    void Start()
    {
        if (!isNonDestroyed)
        {
            textHp = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            sliderHp = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        }

        gameManager = FindAnyObjectByType<GameManager>();
        if(!gameManager.isLevelMission)
            maxEnemies = 2;

        if (gameManager.Difficult == GameManager.difficult.hardcore && !gameManager.isLevelMission)
        {
            maxHealth *= 2;
            maxEnemies = 3;
        }
        currentHealth = maxHealth;
        nextSpawnTime = Time.time;

        rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            // Сохраняем оригинальный цвет
            originalColor = rend.material.color;
        }
    }

    void Update()
    {
        if (!isNonDestroyed)
        {
            textHp.text = $"{currentHealth}/{maxHealth}";
            float a = Mathf.Clamp01(currentHealth / maxHealth);
            sliderHp.fillAmount = a;
        }

        if (isDestroyed) return;

        if (isStart)
        {
            if (Time.time >= nextSpawnTime && spawnedEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    public void Highlight(bool enable)
    {
        if (isDestroyed || rend == null || rend.material == null) return;

        isHighlighted = enable;

        if (enable)
        {
            // Устанавливаем цвет подсветки
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", highlightColor * highlightIntensity);
        }
        else
        {
            // Возвращаем оригинальный цвет
            rend.material.DisableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.black);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)],
                                      spawnPos,
                                      Quaternion.identity,
                                      transform);

        SpawnerEnemyController enemyController = enemy.GetComponent<SpawnerEnemyController>();

        if (enemyController != null)
        {
            // Отключаем физику и коллайдеры на время появления
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            rb.velocity = Vector3.zero; //new Vector3(rb.velocity.x, 0, rb.velocity.z);
            enemy.GetComponent<Collider>().enabled = false;

            enemyController.SetSpawnPoint(this);
            spawnedEnemies.Add(enemyController);
            enemyController.SetPatrolTarget(GetRandomPatrolPoint());
        }
    }

    public Vector3 posCor;
    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPos = transform.position;
        RaycastHit hit;

        // Корректируем позицию спавна на поверхности
        if (Physics.Raycast(transform.position + Vector3.up * 0.9f,
                            Vector3.down,
                            out hit,
                            20f))
        {
            spawnPos = hit.point + posCor;
        }

        return spawnPos;
    }

    public Vector3 GetRandomPatrolPoint()
    {
        // Рассчитываем центр патрулирования с учетом смещения
        Vector3 patrolCenter = transform.position + patrolCenterOffset;

        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        Vector3 patrolPosition = patrolCenter + new Vector3(randomPoint.x, 0, randomPoint.y);

        // Гарантируем, что точка на уровне земли
        RaycastHit hit;
        if (Physics.Raycast(patrolPosition + Vector3.up * 10, Vector3.down, out hit, 20f))
        {
            patrolPosition.y = hit.point.y;
        }

        return patrolPosition;
    }

    public void EnemyDied(SpawnerEnemyController enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroySpawnPoint();
        }
    }

    private void DestroySpawnPoint()
    {
        // Создаем временную копию списка для безопасного перебора
        List<SpawnerEnemyController> enemiesToKill = new List<SpawnerEnemyController>(spawnedEnemies);

        // Убиваем всех врагов из временного списка
        foreach (SpawnerEnemyController enemy in enemiesToKill)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(enemy.maxHealth * 10); // Убиваем врага
            }
        }

        // Очищаем оригинальный список
        spawnedEnemies.Clear();
        isDestroyed = true;

        // Эффект разрушения
        GameObject eff =Instantiate(gameManager.effectMol, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(eff, 1);
        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + posCor, 1f);
        Gizmos.DrawIcon(transform.position + Vector3.up * 2, "SpawnPoint");

        // Рассчитываем центр патрулирования с учетом смещения
        Vector3 patrolCenter = transform.position + patrolCenterOffset;

        // Визуализация радиуса патрулирования
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(patrolCenter, patrolRadius);

        // Визуализация смещения центра
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, patrolCenter);
        Gizmos.DrawWireCube(patrolCenter, Vector3.one * 0.5f);
    }
}