using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemyController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public bool isAlive = true;
    [Range(0, 1)]
    public float damageAnimChance = 0.3f; // 30% шанс на анимацию урона

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.5f;

    [Header("Attack Settings")]
    public int damagePerHit = 25;
    public float minAttackDelay = 1f;
    public float maxAttackDelay = 3f;
    public float minAttackInterval = 2f;
    public float maxAttackInterval = 5f;

    [Header("References")]
    public Animator animator;
    public Renderer enemyRenderer;
    public GameObject deathEffect;

    [Header("Spawning Settings")]
    public GameObject spawnEffect; // Эффект при появлении
    public float emergeDuration = 2f; // Длительность появления из земли
    public float startDepth = 2f; // Начальная глубина под землей

    public Vector3 spawnPosition; // Позиция появления
    private bool isEmerging = true; // Флаг процесса появления

    [Header("Debug Settings")]
    public bool showDebugLogs = false; // Для отладки

    // Ссылки на системы
    private SpawnPoint spawnPoint;
    private Transform playerTransform;
    private GameManager gameManager;
    private Rigidbody rb;

    // Состояния врага
    private enum EnemyState { MovingToPosition, Attacking, Emerging}
    private EnemyState currentState = EnemyState.MovingToPosition;

    // Переменные для движения
    private Vector3 targetPosition;
    private bool isAtPosition = false;

    // Переменные для атаки
    private float nextAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        playerTransform = Camera.main.transform;
        currentHealth = maxHealth;
        if (gameManager.Difficult == GameManager.difficult.hardcore)
        {
            currentHealth = maxHealth * 2;
        }

        // Установка времени следующей атаки
        nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);

        // Начальное состояние - движение к позиции
        currentState = EnemyState.MovingToPosition;

        // Убедимся, что Rigidbody не мешает движению
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        /*if (animator != null)
            animator.SetBool("isRunning", true);*/

        spawnPosition = transform.position;
        currentState = EnemyState.Emerging; // Начинаем с состояния появления

        // Начинаем процесс появления
        StartCoroutine(EmergeFromGround());
    }
    public Vector3 posUp;
    private IEnumerator EmergeFromGround()
    {
        transform.position = spawnPosition - Vector3.up * startDepth;

        // Создаем эффект появления
        if (spawnEffect != null)
        {
            GameObject eff = Instantiate(spawnEffect, spawnPosition - posUp, Quaternion.identity);
            Destroy(eff, emergeDuration);
        }

        // Плавное появление из земли
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < emergeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / emergeDuration;
            transform.position = Vector3.Lerp(startPos, spawnPosition, progress);
            yield return null;
        }

        // Завершение появления
        isEmerging = false;
        currentState = EnemyState.MovingToPosition;
        rb.velocity = Vector3.zero;

        // Включаем физику и коллайдеры
        if (rb != null) rb.isKinematic = false;
        GetComponent<Collider>().enabled = true;
    }


    public void SetSpawnPoint(SpawnPoint point)
    {
        spawnPoint = point;
    }

    public void SetPatrolTarget(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        if (!isAlive || isEmerging) return;

        switch (currentState)
        {
            case EnemyState.MovingToPosition:
                MoveToTargetPosition();
                break;

            case EnemyState.Attacking:
                AttackBehavior();
                break;
        }

        // Отладочная информация
        if (showDebugLogs)
        {
            Debug.Log($"State: {currentState}, Position: {transform.position}, Target: {targetPosition}");
        }
    }


    private void MoveToTargetPosition()
    {
        // Проверяем, установлена ли целевая позиция
        if (targetPosition == Vector3.zero)
        {
            Debug.LogWarning("Target position not set for enemy!");
            return;
        }
        animator.SetBool("isRunning", true);

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= stoppingDistance)
        {
            // Достигли целевой позиции
            isAtPosition = true;
            currentState = EnemyState.Attacking;

            if (animator != null)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdle", true);
            }

            // Останавливаем физическое движение
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            // Движение к целевой позиции
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            // Используем физику для плавного движения
            if (rb != null)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }

            // Поворот к цели
            RotateTowards(targetPosition);

            // Обновляем анимацию
            if (animator != null && !animator.GetBool("isRunning"))
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdle", false);
            }
        }
    }

    public void InstantKill()
    {
        // Если уже мертв, ничего не делаем
        if (!isAlive) return;

        // Немедленная смерть без анимации и эффектов
        isAlive = false;

        // Уведомляем точку спавна о смерти
        if (spawnPoint != null)
        {
            spawnPoint.EnemyDied(this);
        }

        // Немедленно уничтожаем объект
        Destroy(gameObject);
    }

    private void AttackBehavior()
    {
        // Поворачиваемся к игроку
        RotateTowards(playerTransform.position);

        // Атака при готовности
        if (Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // Ограничиваем вращение только по оси Y
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Плавный поворот
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    
    private IEnumerator delayedSpawnBullet()
    {
        yield return new WaitForSeconds(0.6f);
        Instantiate(bullet, pos.position, Quaternion.identity);
    }

    [SerializeField] private GameObject bullet;
    public Transform pos;
    private void Attack()
    {
        isAttacking = true;
        StartCoroutine(delayedSpawnBullet());
        // Запуск анимации атаки
        if (animator != null)
        {
            animator.SetTrigger("isAttack");
        }

        // Нанесение урона игроку
        GameManager.currentHP -= damagePerHit;

        // Запуск корутины для сброса флага атаки
        StartCoroutine(ResetAttackState());

        // Установка времени следующей атаки
        nextAttackTime = Time.time + Random.Range(minAttackInterval, maxAttackInterval);
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive || isEmerging) return;

        // Случайное воспроизведение анимации урона
        if (animator != null && Random.value <= damageAnimChance)
        {
            animator.SetTrigger("TakeDamage");
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Анимация получения урона
            if (animator != null)
                animator.SetTrigger("isHit");
        }
    }

    private void Die()
    {
        isAlive = false;

        // Уведомляем точку спавна о смерти
        if (spawnPoint != null)
        {
            spawnPoint.EnemyDied(this);
        }

        // Анимация смерти
        if (animator != null)
            animator.SetBool("isDie", true);

        // Эффект смерти
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.Euler(-90, 0, 0));

        // Отключаем физику через время
        StartCoroutine(DisablePhysics(1.4f));

        // Удаление объекта с задержкой
        Destroy(gameObject, 3f);
    }

    private IEnumerator DisablePhysics(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null) rb.useGravity = true;
        GetComponent<CapsuleCollider>().height = 1;
        GetComponent<CapsuleCollider>().center = new Vector3(0, 1.2f, 0);
        //if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
    }

    public void Highlight(bool enable)
    {
        if (enemyRenderer == null || !isAlive) return;

        if (enable)
        {
            // Подсветка при наведении
            foreach (var mat in enemyRenderer.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.red * 0.5f);
            }
        }
        else
        {
            // Убираем подсветку
            foreach (var mat in enemyRenderer.materials)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }
}
