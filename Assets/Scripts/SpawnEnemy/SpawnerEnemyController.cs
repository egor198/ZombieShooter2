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
    public float damageAnimChance = 0.3f; // 30% ���� �� �������� �����

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
    public GameObject spawnEffect; // ������ ��� ���������
    public float emergeDuration = 2f; // ������������ ��������� �� �����
    public float startDepth = 2f; // ��������� ������� ��� ������

    public Vector3 spawnPosition; // ������� ���������
    private bool isEmerging = true; // ���� �������� ���������

    [Header("Debug Settings")]
    public bool showDebugLogs = false; // ��� �������

    // ������ �� �������
    private SpawnPoint spawnPoint;
    private Transform playerTransform;
    private GameManager gameManager;
    private Rigidbody rb;

    // ��������� �����
    private enum EnemyState { MovingToPosition, Attacking, Emerging}
    private EnemyState currentState = EnemyState.MovingToPosition;

    // ���������� ��� ��������
    private Vector3 targetPosition;
    private bool isAtPosition = false;

    // ���������� ��� �����
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

        // ��������� ������� ��������� �����
        nextAttackTime = Time.time + Random.Range(minAttackDelay, maxAttackDelay);

        // ��������� ��������� - �������� � �������
        currentState = EnemyState.MovingToPosition;

        // ��������, ��� Rigidbody �� ������ ��������
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        /*if (animator != null)
            animator.SetBool("isRunning", true);*/

        spawnPosition = transform.position;
        currentState = EnemyState.Emerging; // �������� � ��������� ���������

        // �������� ������� ���������
        StartCoroutine(EmergeFromGround());
    }
    public Vector3 posUp;
    private IEnumerator EmergeFromGround()
    {
        transform.position = spawnPosition - Vector3.up * startDepth;

        // ������� ������ ���������
        if (spawnEffect != null)
        {
            GameObject eff = Instantiate(spawnEffect, spawnPosition - posUp, Quaternion.identity);
            Destroy(eff, emergeDuration);
        }

        // ������� ��������� �� �����
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < emergeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / emergeDuration;
            transform.position = Vector3.Lerp(startPos, spawnPosition, progress);
            yield return null;
        }

        // ���������� ���������
        isEmerging = false;
        currentState = EnemyState.MovingToPosition;
        rb.velocity = Vector3.zero;

        // �������� ������ � ����������
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

        // ���������� ����������
        if (showDebugLogs)
        {
            Debug.Log($"State: {currentState}, Position: {transform.position}, Target: {targetPosition}");
        }
    }


    private void MoveToTargetPosition()
    {
        // ���������, ����������� �� ������� �������
        if (targetPosition == Vector3.zero)
        {
            Debug.LogWarning("Target position not set for enemy!");
            return;
        }
        animator.SetBool("isRunning", true);

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= stoppingDistance)
        {
            // �������� ������� �������
            isAtPosition = true;
            currentState = EnemyState.Attacking;

            if (animator != null)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdle", true);
            }

            // ������������� ���������� ��������
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            // �������� � ������� �������
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            // ���������� ������ ��� �������� ��������
            if (rb != null)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }

            // ������� � ����
            RotateTowards(targetPosition);

            // ��������� ��������
            if (animator != null && !animator.GetBool("isRunning"))
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdle", false);
            }
        }
    }

    public void InstantKill()
    {
        // ���� ��� �����, ������ �� ������
        if (!isAlive) return;

        // ����������� ������ ��� �������� � ��������
        isAlive = false;

        // ���������� ����� ������ � ������
        if (spawnPoint != null)
        {
            spawnPoint.EnemyDied(this);
        }

        // ���������� ���������� ������
        Destroy(gameObject);
    }

    private void AttackBehavior()
    {
        // �������������� � ������
        RotateTowards(playerTransform.position);

        // ����� ��� ����������
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
            // ������������ �������� ������ �� ��� Y
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // ������� �������
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
        // ������ �������� �����
        if (animator != null)
        {
            animator.SetTrigger("isAttack");
        }

        // ��������� ����� ������
        GameManager.currentHP -= damagePerHit;

        // ������ �������� ��� ������ ����� �����
        StartCoroutine(ResetAttackState());

        // ��������� ������� ��������� �����
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

        // ��������� ��������������� �������� �����
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
            // �������� ��������� �����
            if (animator != null)
                animator.SetTrigger("isHit");
        }
    }

    private void Die()
    {
        isAlive = false;

        // ���������� ����� ������ � ������
        if (spawnPoint != null)
        {
            spawnPoint.EnemyDied(this);
        }

        // �������� ������
        if (animator != null)
            animator.SetBool("isDie", true);

        // ������ ������
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.Euler(-90, 0, 0));

        // ��������� ������ ����� �����
        StartCoroutine(DisablePhysics(1.4f));

        // �������� ������� � ���������
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
            // ��������� ��� ���������
            foreach (var mat in enemyRenderer.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.red * 0.5f);
            }
        }
        else
        {
            // ������� ���������
            foreach (var mat in enemyRenderer.materials)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }
}
