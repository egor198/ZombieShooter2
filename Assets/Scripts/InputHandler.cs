using ArtlessUI.Ui.SubPanels;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler: MonoBehaviour
{
    [Header("Settings")]
    public LayerMask enemyLayer;
    public int baseDamage = 25;
    public bool highlightEnabled = true;
    public float attackInterval = 0.2f;
    public int maxTouches = 5; // ������������ ���������� ������������� �������

    [Header("Critical Hit Settings")]
    public LayerMask criticalHitLayer;
    public LayerMask bonus;
    public LayerMask spawnPointLayer;
    public LayerMask CameraEnemyLayer;

    [Header("Sound Settings")]
    public AudioClip enemyHitSound;
    public AudioClip criticalHitSound;
    public AudioClip spawnPointHitSound;
    public AudioClip firstAidSound;
    public AudioClip explosionSound;
    private AudioSource audioSource;

    [SerializeField] private GameObject effectClickEnemy;
    [SerializeField] private GameObject effectClickBonus;
    [SerializeField] private GameObject effectClickSpawnPoint;
    [SerializeField] private GameObject effectFireBonus;
    [SerializeField] private GameObject effectIceBonus;

    private Camera mainCamera;
    private SpawnerEnemyController lastHighlightedEnemy;
    private EnemyController lastHighlighteEn;
    private FirstAidKitController lastHighlightedFirstAid;
    private SpawnPoint lastHighlightedSpawnPoint;
    private float nextAttackTime;
    private Dictionary<int, float> lastTouchTimes = new Dictionary<int, float>(); // ����� ��������� ����� ��� ������� �������

    void Start()
    {
        mainCamera = Camera.main;
        nextAttackTime = Time.time;
        // ������������� �����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ��������� ������ �� ������� ������/����
        HighlightEnemyUnderCursor();

        // ��������� ����������
        ProcessMultiTouch();
    }

    private void ProcessMultiTouch()
    {
        // ��������� ���� (��� ���������)
        if (Input.GetMouseButton(0))
        {
            ProcessTouch(-1, Input.mousePosition);
        }

        // ��������� �������
        for (int i = 0; i < Mathf.Min(Input.touchCount, maxTouches); i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                ProcessTouch(touch.fingerId, touch.position);
            }
        }
    }

    private void ProcessTouch(int touchId, Vector2 position)
    {
        // ������������� ������� ��� ������ �������
        if (!lastTouchTimes.ContainsKey(touchId))
        {
            lastTouchTimes[touchId] = 0f;
        }

        // �������� ��������� ����� ��� ����� �������
        if (Time.time - lastTouchTimes[touchId] >= attackInterval)
        {
            HandleTap(position);
            lastTouchTimes[touchId] = Time.time;
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        bool hitProcessed = false;

        // 1. �������� ��������� � ����������� ����
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit criticalHit, Mathf.Infinity, criticalHitLayer))
        {
            CriticalHitArea criticalArea = criticalHit.collider.GetComponent<CriticalHitArea>();
            PlaySound(criticalHitSound);
            if (criticalArea != null && criticalArea.enemyController != null &&
                criticalArea.enemyController.CanBeAttacked())
            {
                int damage = Mathf.RoundToInt(baseDamage * criticalArea.damageMultiplier);
                GameManager.score += 50;
                GameManager.comboCrit += 1;
                if (effectClickEnemy != null) Instantiate(effectClickEnemy, criticalHit.point, Quaternion.identity);
                criticalArea.enemyController.TakeDamage(damage);
                hitProcessed = true;
            }
        }

        // 2. �������� ��������� � ������� ����� ����� (SpawnerEnemy)
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyLayer))
        {
            SpawnerEnemyController enemy = hit.collider.GetComponent<SpawnerEnemyController>();
            PlaySound(enemyHitSound);
            if (enemy != null && enemy.isAlive)
            {
                GameManager.score += 25;
                GameManager.comboCrit = 0;
                if(GameManager.isFireBonus)
                {
                    enemy.TakeDamage(enemy.maxHealth);
                    Instantiate(effectFireBonus, hit.point, Quaternion.identity);
                }
                else if (GameManager.isIceBonus)
                {
                    enemy.TakeDamage(enemy.maxHealth);
                    Instantiate(effectIceBonus, hit.point, Quaternion.identity);
                }
                else
                {
                    enemy.TakeDamage(baseDamage);
                    Instantiate(effectClickEnemy, hit.point, Quaternion.identity);
                }
                
                hitProcessed = true;
            }
        }

        // 3. �������� ��������� � ������� ����� ����� (EnemyController)
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit Enemyhit, Mathf.Infinity, enemyLayer))
        {
            EnemyController enemy = Enemyhit.collider.GetComponent<EnemyController>();
            PlaySound(enemyHitSound);
            if (enemy != null && enemy.isAlive)
            {
                GameManager.score += 25;
                GameManager.comboCrit = 0;
                if (GameManager.isFireBonus)
                {
                    enemy.TakeDamage(enemy.maxHealth);
                    Instantiate(effectFireBonus, Enemyhit.point, Quaternion.identity);
                }
                else if (GameManager.isIceBonus)
                {
                    enemy.TakeDamage(enemy.maxHealth);
                    Instantiate(effectIceBonus, Enemyhit.point, Quaternion.identity);
                }
                else
                {
                    enemy.TakeDamage(baseDamage);
                    Instantiate(effectClickEnemy, Enemyhit.point, Quaternion.identity);
                }
                hitProcessed = true;
            }
        }

        // 4. �������� ��������� � ����� ������
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit spawnPointHit, Mathf.Infinity, spawnPointLayer))
        {
            SpawnPoint spawnPoint = spawnPointHit.collider.GetComponent<SpawnPoint>();
            PlaySound(spawnPointHitSound);
            if (spawnPoint != null && !spawnPoint.isDestroyed)
            {
                GameManager.score += 30;
                if (effectClickSpawnPoint != null)
                    Instantiate(effectClickSpawnPoint, spawnPointHit.point, Quaternion.identity);
                spawnPoint.TakeDamage(baseDamage);
                hitProcessed = true;
            }
        }

        // 5. �������� ��������� � �������
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit bonusHit, Mathf.Infinity, bonus))
        {
            if (bonusHit.transform.gameObject.TryGetComponent(out TNTController TNT))
            {
                if (effectClickBonus != null) Instantiate(effectClickBonus, bonusHit.point, Quaternion.identity);
                PlaySound(explosionSound);
                TNT.Explosion();
                hitProcessed = true;
            }
            if (bonusHit.transform.tag == "Box")
            {
                Destroy(bonusHit.transform.gameObject);
                GameManager.currentBox++;
            }
            if (bonusHit.transform.tag == "FireBonus")
            {
                Destroy(bonusHit.transform.gameObject);
                GameManager.isFireBonus = true;
            }
            if (bonusHit.transform.tag == "IceBonus")
            {
                Destroy(bonusHit.transform.gameObject);
                GameManager.isIceBonus = true;
            }
            if (bonusHit.transform.gameObject.TryGetComponent(out FirstAidKitController FirstAidKit) && !bonusHit.transform.gameObject.TryGetComponent(out TNTController TNTNo) && bonusHit.transform.tag == "Aid")
            {
                FirstAidKit.AddHp();
                PlaySound(firstAidSound);
                if (effectClickBonus != null) Instantiate(effectClickBonus, bonusHit.point, Quaternion.identity);
                FirstAidKit.Highlight(true);
                hitProcessed = true;
            }
        }
        if (!hitProcessed && Physics.Raycast(ray, out RaycastHit HitCameraEnemy, Mathf.Infinity, CameraEnemyLayer))
        {
            EnemyCamera enem = HitCameraEnemy.transform.GetComponent<EnemyCamera>();
            enem.HideObject();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void HighlightEnemyUnderCursor()
    {
        if (!highlightEnabled)
        {
            ClearHighlight();
            return;
        }

        // ���������� ������� ������� ������ ��� ���� ��� ���������
        Vector2 screenPosition = Input.mousePosition;
        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        // �������� ����������� ���
        RaycastHit criticalHit;
        if (Physics.Raycast(ray, out criticalHit, Mathf.Infinity, criticalHitLayer))
        {
            CriticalHitArea criticalArea = criticalHit.collider.GetComponent<CriticalHitArea>();
            if (criticalArea != null && criticalArea.enemyController != null)
            {
                //HighlightEnemy(criticalArea.enemyController);
                return;
            }
        }

        // �������� ������� ������
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            SpawnerEnemyController enemy = hit.collider.GetComponent<SpawnerEnemyController>();
            if (enemy != null && enemy.isAlive)
            {
                HighlightEnemy(enemy);
                return;
            }
        }

        // �������� ������� ������
        RaycastHit Enemyhit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.isAlive)
            {
                HighlightEnemy2(enemy);
                return;
            }
        }

        // �������� ����� ������
        RaycastHit spawnPointHit;
        if (Physics.Raycast(ray, out spawnPointHit, Mathf.Infinity, spawnPointLayer))
        {
            SpawnPoint spawnPoint = spawnPointHit.collider.GetComponent<SpawnPoint>();
            if (spawnPoint != null && !spawnPoint.isDestroyed)
            {
                // ��������� ����� ������
                HighlightSpawnPoint(spawnPoint);
                return;
            }
        }

        // �������� �������
        RaycastHit bonusHit;
        if (Physics.Raycast(ray, out bonusHit, Mathf.Infinity, bonus))
        {
            FirstAidKitController firstAid = bonusHit.collider.GetComponent<FirstAidKitController>();
            if (firstAid != null)
            {
                if (lastHighlightedFirstAid != null && lastHighlightedFirstAid != firstAid)
                {
                    lastHighlightedFirstAid.Highlight(false);
                }
                firstAid.Highlight(true);
                lastHighlightedFirstAid = firstAid;
                return;
            }
        }

        ClearHighlight();
    }


    // ��������� ����� ������
    private void HighlightSpawnPoint(SpawnPoint spawnPoint)
    {
        if (spawnPoint != lastHighlightedSpawnPoint)
        {
            // ������� ��������� � ���������� ����� ������
            if (lastHighlightedSpawnPoint != null)
            {
                lastHighlightedSpawnPoint.Highlight(false);
            }

            // ������������ ����� �����
            spawnPoint.Highlight(true);
            lastHighlightedSpawnPoint = spawnPoint;
        }

        // ������� ��������� � ������ ��������
        if (lastHighlightedEnemy != null)
        {
            lastHighlightedEnemy.Highlight(false);
            lastHighlightedEnemy = null;
        }
        if (lastHighlightedFirstAid != null)
        {
            lastHighlightedFirstAid.Highlight(false);
            lastHighlightedFirstAid = null;
        }
    }

    private void HighlightEnemy(SpawnerEnemyController enemy)
    {
        if (enemy != lastHighlightedEnemy)
        {
            ClearHighlight();
            enemy.Highlight(true);
            lastHighlightedEnemy = enemy;
        }
    }

    private void HighlightEnemy2(EnemyController enemy)
    {
        if (enemy != lastHighlightedEnemy)
        {
            ClearHighlight();
            enemy.Highlight(true);
            lastHighlighteEn = enemy;
        }
    }

    private void ClearHighlight()
    {
        if (lastHighlightedEnemy != null)
        {
            lastHighlightedEnemy.Highlight(false);
            lastHighlightedEnemy = null;
        }
        if (lastHighlighteEn != null)
        {
            lastHighlighteEn.Highlight(false);
            lastHighlighteEn = null;
        }
        if (lastHighlightedFirstAid != null)
        {
            lastHighlightedFirstAid.Highlight(false);
            lastHighlightedFirstAid = null;
        }
        if (lastHighlightedSpawnPoint != null)
        {
            lastHighlightedSpawnPoint.Highlight(false);
            lastHighlightedSpawnPoint = null;
        }
    }

    void OnDestroy()
    {
        ClearHighlight();
    }
}
