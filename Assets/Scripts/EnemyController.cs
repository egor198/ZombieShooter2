using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100;
    public float currentHealth;
    public bool isAlive = true;
    [Range(0, 1)]
    public float damageAnimChance = 0.3f; // 30% шанс на анимацию урона
    //public HealthBar healthBar; // Опционально

    [Header("Approach Settings")]
    public float minDistanceToCamera = 3f; // Минимальная дистанция к камере
    public float approachDistance = 2f; // Дистанция приближения после атаки

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public float cameraRotationSpeed = 2f;
    public float rotationToCameraThreshold = 1f;

    [Header("Attack Settings")]
    public int damagePerHit = 25;

    [Header("Death Settings")]
    public float destroyDelay = 2f;
    public GameObject deathEffect; // Опционально

    [Header("References")]
    public Animator animator;
    public Renderer enemyRenderer; // Для подсветки при наведении

    [Header("Critical Hit Settings")]
    public List<CriticalHitArea> criticalAreas = new List<CriticalHitArea>();

    [Header("Attack Settings")]
    public float minAttackDelay = 1f; // Минимальная задержка перед первой атакой
    public float maxAttackDelay = 3f; // Максимальная задержка перед первой атакой
    public float minAttackInterval = 2f; // Минимальный интервал между атаками
    public float maxAttackInterval = 5f; // Максимальный интервал между атаками
    public int damage;
    [SerializeField] private GameObject bullet;
    public Transform pos;

    [Header("Boss Settings")]
    public bool isBoss = false; // Новая переменная для определения босса
    public float bossMoveDelay = 1f; // Задержка перед перемещением босса после атаки
    public TextMeshProUGUI textSliderHp;
    public Image sliderHp;

    [Header("Sound Settings")]
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    public AudioClip attackSound;
    public AudioClip moveSound;
    private AudioSource audioSource;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    public Waypoint targetWaypoint;
    private bool isActive = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public EnemyManager manager;
    private Camera mainCamera;
    private bool isRotatingToCamera = false;
    private Quaternion targetCameraRotation;
    private bool isHighlighted = false;
    private bool isAtWaypoint = false; // Новый флаг

    private Rigidbody rb;
    private CapsuleCollider colider;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colider = GetComponent<CapsuleCollider>();
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager.Difficult == GameManager.difficult.hardcore)
        {
            maxHealth *= 2;
        }
        currentHealth = maxHealth;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //manager = FindObjectOfType<EnemyManager>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        //if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        if (enemyRenderer == null) enemyRenderer = GetComponentInChildren<Renderer>();

        criticalAreas.AddRange(GetComponentsInChildren<CriticalHitArea>());

        nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);
    }

    void Update()
    {
        if (!isAlive) return;

        // Атака при достижении точки
        if (isAtWaypoint && !isRotatingToCamera && isReadyToAttack())
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        if (mainCamera != null && isAtWaypoint)
        {
            float distanceToCamera = Vector3.Distance(
                transform.position,
                mainCamera.transform.position
            );

            if (distanceToCamera <= minDistanceToCamera)
            {
                // Слишком близко к камере - не пытаемся приближаться дальше
                return;
            }
        }

        if (isRotatingToCamera)
        {
            RotateToCamera();
            if (isBoss)
            {
                sliderHp.gameObject.SetActive(true);
                textSliderHp.text = $"{currentHealth}/{maxHealth}";
                float slider = Mathf.Clamp01(currentHealth / maxHealth);
                sliderHp.fillAmount = slider;
            }
            return;
        }

        if (!isActive || targetWaypoint == null) return;

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Vector3 targetPosition = targetWaypoint.transform.position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Воспроизведение звука движения
        if (!audioSource.isPlaying && moveSound != null)
        {
            audioSource.clip = moveSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (distance <= stoppingDistance)
        {
            // Точка уже занята этим врагом, просто устанавливаем флаги
            isActive = false;
            isAtWaypoint = true;

            PrepareCameraRotation();
        }
        else
        {
            // Движение к точке
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Поворот к цели
            RotateTowards(targetPosition);
        }
    }


    private void RotateTowards(Vector3 targetPosition)
    {
        // Рассчитываем направление к цели
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Игнорируем вертикальную составляющую для вращения только по Y
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Создаем целевой поворот только для оси Y
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Сохраняем текущие углы поворота по X и Z
            Vector3 euler = transform.rotation.eulerAngles;

            // Плавно изменяем только угол Y
            Quaternion newRotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Комбинируем новый Y с сохраненными X и Z
            transform.rotation = Quaternion.Euler(
                euler.x,
                newRotation.eulerAngles.y,
                euler.z
            );
        }
    }

    private bool MoveCloserToCamera()
    {
        if (mainCamera == null) return false;

        // Вычисляем направление к камере
        Vector3 directionToCamera = (mainCamera.transform.position - transform.position).normalized;

        // Вычисляем новую позицию (приближаемся к камере, но не ближе minDistanceToCamera)
        Vector3 targetPosition = transform.position + directionToCamera * approachDistance;

        // Проверяем дистанцию до камеры
        float distanceToCamera = Vector3.Distance(targetPosition, mainCamera.transform.position);
        if (distanceToCamera < minDistanceToCamera)
        {
            // Не подходим ближе минимальной дистанции
            targetPosition = mainCamera.transform.position - directionToCamera * minDistanceToCamera;
        }

        // Создаем временный Waypoint для новой позиции
        GameObject tempWaypoint = new GameObject("TempWaypoint");
        tempWaypoint.transform.position = targetPosition;
        Waypoint waypointComponent = tempWaypoint.AddComponent<Waypoint>();

        // Освобождаем текущую точку
        if (targetWaypoint != null)
        {
            targetWaypoint.isOccupied = false;
            targetWaypoint.occupiedBy = null;
        }

        // Занимаем новую позицию
        if (waypointComponent.TryOccupy(this))
        {
            targetWaypoint = waypointComponent;
            isActive = true;
            isAtWaypoint = false;
            isRotatingToCamera = false;

            nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);

            if (animator != null)
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("isRunning", true);
            }

            // Уничтожаем временный Waypoint после использования
            Destroy(tempWaypoint, 5f);
            return true;
        }

        Destroy(tempWaypoint);
        return false;
    }

    // Проверка готовности к атаке
    private bool isReadyToAttack()
    {
        return isAtWaypoint && !isRotatingToCamera && !isAttacking;
    }

    private void PrepareCameraRotation()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Остановка звука движения
        if (audioSource.isPlaying && audioSource.clip == moveSound)
        {
            audioSource.Stop();
        }

        Vector3 toCamera = mainCamera.transform.position - transform.position;
        toCamera.y = 0;

        if (toCamera != Vector3.zero)
        {
            targetCameraRotation = Quaternion.LookRotation(toCamera);
            isRotatingToCamera = true;
        }
    }

    private void MoveBossToNewPosition()
    {
        if (manager == null || manager.waypoints.Count == 0) return;

        // Освобождаем текущую точку
        if (targetWaypoint != null)
        {
            targetWaypoint.isOccupied = false;
            targetWaypoint.occupiedBy = null;
        }

        // Выбираем случайную новую точку
        Waypoint newWaypoint = GetRandomWaypoint();
        if (newWaypoint == null) return;

        // Занимаем новую точку
        if (newWaypoint.TryOccupy(this))
        {
            targetWaypoint = newWaypoint;
            isActive = true;
            isAtWaypoint = false;
            isRotatingToCamera = false;

            // Сбрасываем таймер атаки
            nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);

            if (animator != null)
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("isRunning", true);
            }
        }
    }

    private Waypoint GetRandomWaypoint()
    {
        if (manager == null || manager.waypoints.Count == 0) return null;

        // Создаем список доступных точек
        List<Waypoint> availableWaypoints = new List<Waypoint>();
        foreach (Waypoint waypoint in manager.waypoints)
        {
            if (waypoint != targetWaypoint && !waypoint.isOccupied)
            {
                availableWaypoints.Add(waypoint);
            }
        }

        // Если нет доступных точек, используем любую (кроме текущей)
        if (availableWaypoints.Count == 0)
        {
            foreach (Waypoint waypoint in manager.waypoints)
            {
                if (waypoint != targetWaypoint)
                {
                    availableWaypoints.Add(waypoint);
                }
            }
        }

        // Если все равно нет точек, возвращаем null
        if (availableWaypoints.Count == 0) return null;

        // Выбираем случайную точку
        int randomIndex = Random.Range(0, availableWaypoints.Count);
        return availableWaypoints[randomIndex];
    }


    private void RotateToCamera()
    {
        if (Quaternion.Angle(transform.rotation, targetCameraRotation) < rotationToCameraThreshold)
        {
            isRotatingToCamera = false;
            if (animator != null) animator.SetBool("isIdle", true);
            animator.SetBool("isRunning", false);
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetCameraRotation,
            cameraRotationSpeed * Time.deltaTime
        );
    }

    // Изменено: теперь можно атаковать в любое время
    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        // Звук получения урона
        if (takeDamageSound != null)
        {
            audioSource.PlayOneShot(takeDamageSound);
        }

        // Случайное воспроизведение анимации урона
        if (animator != null && Random.value <= damageAnimChance)
        {
            animator.SetTrigger("TakeDamage");
        }

        currentHealth -= damage;
        //if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (isBoss)
        {
            sliderHp.gameObject.SetActive(true);
            textSliderHp.text = $"{currentHealth}/{maxHealth}";
            float slider = Mathf.Clamp01(currentHealth / maxHealth);
            sliderHp.fillAmount = slider;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;

        if (isBoss)
            sliderHp.gameObject.SetActive(false);

        // Звук смерти
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Освобождаем точку атомарно
        if (targetWaypoint != null && targetWaypoint.occupiedBy == this)
        {
            targetWaypoint.isOccupied = false;
            targetWaypoint.occupiedBy = null;
            Debug.Log($"Waypoint {targetWaypoint.name} released by {name}");
        }

        // Останавливаем движение
        isActive = false;
        isAtWaypoint = false;

        // Анимация смерти
        if (animator != null) animator.SetBool("isDie", true);

        // Запускаем корутину с задержкой
        StartCoroutine(DelayedPhysicsChanges(1.4f)); // 1 секунда задержки

        // Эффект смерти
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.Euler(-90, 0, 0));

        // Удаление объекта с задержкой
        Destroy(gameObject, destroyDelay);
        manager.RemoveEnemy(this);

        // Освобождаем точку безопасным способом
        if (targetWaypoint != null && targetWaypoint.occupiedBy == this)
        {
            targetWaypoint.isOccupied = false;
            targetWaypoint.occupiedBy = null;
        }

        isAttacking = false;
        //StopAllCoroutines();

        // Убираем из списка врагов
        //if (manager != null) manager.RemoveEnemy(this);
    }

    // Метод атаки
    private void Attack()
    {
        isAttacking = true;
        StartCoroutine(delayedSpawnBullet());

        // Звук атаки
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Запуск анимации атаки
        if (animator != null)
        {
            animator.SetTrigger("isAttack");
        }

        GameManager.currentHP -= damage;
        // Запуск корутины для сброса флага атаки
        StartCoroutine(ResetAttackState());

        // Установка времени следующей атаки
        nextAttackTime = Time.time + Random.Range(minAttackInterval, maxAttackInterval);
    }

    private IEnumerator delayedSpawnBullet()
    {
        yield return new WaitForSeconds(0.6f);
        Instantiate(bullet, pos.position, Quaternion.identity);
    }

    // Корутина для сброса состояния атаки
    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

        if (isAlive && isAtWaypoint)
        {
            // Для всех врагов: пытаемся приблизиться к камере
            bool moved = MoveCloserToCamera();
            if (!moved)
            {
                // Не удалось приблизиться - остаемся на месте
            }
        }
    }

    private IEnumerator DelayedPhysicsChanges(float delay)
    {
        // Ждем указанное время
        yield return new WaitForSeconds(delay);

        // Включаем гравитацию и меняем коллайдер
        rb.useGravity = true;
        colider.height = 1;
        colider.center = new Vector3(0, 1.2f, 0);
    }

    public void ActivateEnemy()
    {
        if (isActive || !isAlive) return;

        int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts && !isActive)
        {
            Waypoint potentialWaypoint = manager.GetRandomFreeWaypoint();

            if (potentialWaypoint == null)
            {
                Debug.LogWarning("No free waypoints available");
                return;
            }

            // Атомарная попытка занять точку
            if (potentialWaypoint.TryOccupy(this))
            {
                targetWaypoint = potentialWaypoint;
                isActive = true;
                isAtWaypoint = false;
                isRotatingToCamera = false;
                Debug.Log($"Enemy {name} occupied waypoint {targetWaypoint.name}");
                return;
            }

            attempts++;
        }

        Debug.LogWarning($"{name} failed to find free waypoint after {maxAttempts} attempts");
    }

    public float GetDamageMultiplierForCollider(Collider hitCollider)
    {
        CriticalHitArea criticalArea = hitCollider.GetComponent<CriticalHitArea>();
        if (criticalArea != null)
        {
            return criticalArea.damageMultiplier;
        }
        return 1f; // Обычный урон
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

    public void ResetEnemy()
    {
        // Освобождаем точку атомарно
        if (targetWaypoint != null && targetWaypoint.occupiedBy == this)
        {
            targetWaypoint.isOccupied = false;
            targetWaypoint.occupiedBy = null;
        }

        isActive = false;
        isAtWaypoint = false;
        isRotatingToCamera = false;
        targetWaypoint = null;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        Highlight(false);

        // Восстановление здоровья
        currentHealth = maxHealth;

        isAttacking = false;
        nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);
        //if (healthBar != null) healthBar.SetHealth(currentHealth);
    }

    // Новый метод: проверка, можно ли атаковать врага
    public bool CanBeAttacked()
    {
        return isAlive && (isActive || isAtWaypoint);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Explosion"))
        {
            TakeDamage(maxHealth);
        }
    }
}