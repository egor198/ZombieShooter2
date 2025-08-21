using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BezierWalkerWithSpeed cameraMove;
    public BezierSpline[] splines;

    [Header("ManagerPoint")]
    public EnemyManager[] enemyController;
    private int currentPoint = 0;

    [Header("ManagerSpawnPoint")]
    public bool isSpawnPointLevel;
    public ManagerSpawnPoint[] spanwManagers;
    public GameObject[] spawnStone;

    [Header("ManagerLevelMission")]
    public bool isLevelMission;
    public GameObject[] spanwsEnemy;
    public GameObject allBoxPoint2;

    [Header("Bonus")]
    public static bool isFireBonus;
    public static bool isIceBonus;
    private float timerDuration;

    [Header("HP")]
    public float maxHp;
    public GameObject effectMol;
    public static float currentHP;

    [Header("SettingsLevel")]
    public difficult Difficult;
    public enum difficult { defalt, hardcore };
    public static int score;
    public static int comboCrit;

    // Добавляем флаг для отслеживания активации текущего уровня
    private bool isCurrentLevelActivated = false;
    private bool isOne;

    private float currentTime;
    private bool isInitialized = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textCombo;
    [SerializeField] private TextMeshProUGUI textHP;
    [SerializeField] private Image sliderHP;
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelPause;
    [SerializeField] private Image timerImageFire;
    [SerializeField] private Image timerImageIce;
    [SerializeField] private GameObject panelEffectFire;
    [SerializeField] private GameObject panelEffectIce;
    [SerializeField] private TextMeshProUGUI textScoreAndTimer;

    private float timerPoint1;
    public static int currentBox;

    private void Start()
    {
        timerDuration = 10;
        if(PlayerPrefs.GetInt("Dif") == 0)
            Difficult = difficult.defalt;
        else
            Difficult = difficult.hardcore;

        maxHp = PlayerPrefs.GetInt("Hp");
        currentHP = maxHp;
        Time.timeScale = 1;
        if (isLevelMission) return;
        foreach (var manager in enemyController)
        {
            manager.ActDeactEnemy(false);
        }
    }

    private void Update()
    {
        if(isFireBonus || isIceBonus)
        {
            if (!isInitialized)
            {
                InitializeTimer();
            }
            currentTime -= Time.deltaTime;
            if(isFireBonus) timerImageFire.fillAmount = Mathf.Clamp01(currentTime / timerDuration);
            else timerImageIce.fillAmount = Mathf.Clamp01(currentTime / timerDuration);
            if(currentTime <= 0)
            {
                if (isFireBonus)
                {
                    isFireBonus = false;
                    timerImageFire.gameObject.SetActive(false);
                    panelEffectFire.SetActive(false);
                }
                else
                {
                    isIceBonus = false;
                    timerImageIce.gameObject.SetActive(false);
                    panelEffectIce.SetActive(false);
                }
                isInitialized = false;
            }
        }

        textCombo.text = $"Комбо: {comboCrit}";
        textScore.text = $"Очки: {score}";
        textHP.text = $"{currentHP}/{maxHp}";
        float fillValue = Mathf.Clamp01(currentHP / maxHp);
        sliderHP.fillAmount = fillValue;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelPause.activeInHierarchy)
            {
                panelPause.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                panelPause.SetActive(true);
                Time.timeScale = 0;
            }
        }
        if (currentHP <= 0)
        {
            panelGameOver.SetActive(true);
            currentHP = 0;
            Time.timeScale = 0;
        }

        if (isLevelMission)
        {
            LevelMission();
            return;
        }

        if(!isSpawnPointLevel && currentPoint == splines.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1;
            currentPoint = 0;
            cameraMove.NormalizedT = 0;
            comboCrit = 0;
            score = 0;
        }
        else if(isSpawnPointLevel && currentPoint == spanwManagers.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1;
            currentPoint = 0;
            cameraMove.NormalizedT = 0;
            comboCrit = 0;
            score = 0;
        }
  
        if (!isSpawnPointLevel && (!panelWin.activeInHierarchy || !panelGameOver.activeInHierarchy))
        {
            // Проверяем что не достигли конца пути
            if (cameraMove.NormalizedT == 1) return;

            // Проверяем достижение 85% для текущего уровня
            if (cameraMove.NormalizedT >= 0.85f && !isCurrentLevelActivated)
            {
                ActivateCurrentLevel();
            }
        }
        else if(isSpawnPointLevel && (!panelWin.activeInHierarchy || !panelGameOver.activeInHierarchy))
        {
            if (currentPoint < spanwManagers.Length && spanwManagers[currentPoint].isDestroyd() && spanwManagers[currentPoint].gameObject.activeInHierarchy)
            {
                StartCoroutine(delayed());
                isOne = true;
            }
            if (cameraMove.NormalizedT == 1) return;
            if (cameraMove.NormalizedT >= 0.85f)
            {
                spawnStone[currentPoint].SetActive(false);
                spanwManagers[currentPoint].gameObject.SetActive(true);
            }
        }
    }

    void LevelMission()
    {
        if (isLevelMission)
        {
            if (cameraMove.NormalizedT >= 0.6f && cameraMove.spline == splines[0])
            {
                spanwsEnemy[0].SetActive(true);
                timerPoint1 += Time.deltaTime;
                textScoreAndTimer.gameObject.SetActive(true);
                textScoreAndTimer.text = Mathf.Round(timerPoint1).ToString();
                if(timerPoint1 >= 40)
                {
                    spanwsEnemy[0].SetActive(false);
                    cameraMove.spline = splines[1];
                    cameraMove.NormalizedT = 0;
                    timerPoint1 = 0;
                }
            }
            if(cameraMove.NormalizedT >= 0.6f && cameraMove.spline == splines[1])
            {
                allBoxPoint2.SetActive(true);
                textScoreAndTimer.text = $"{currentBox}/9";
                if(currentBox >= 9)
                {
                    allBoxPoint2.SetActive(false);
                    cameraMove.spline = splines[2];
                    cameraMove.NormalizedT = 0;
                }
            }
            if (cameraMove.NormalizedT >= 0.6f && cameraMove.spline == splines[2])
            {
                spanwsEnemy[1].SetActive(true);
                timerPoint1 += Time.deltaTime;
                textScoreAndTimer.text = Mathf.Round(timerPoint1).ToString();
                if (timerPoint1 >= 40)
                {
                    spanwsEnemy[1].SetActive(false);
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    void InitializeTimer()
    {
        currentTime = timerDuration;
        if (isFireBonus) 
        {
            timerImageFire.fillAmount = 1f;
            timerImageFire.gameObject.SetActive(true);
            panelEffectFire.SetActive(true);
        }
        else
        {
            timerImageIce.fillAmount = 1f;
            timerImageIce.gameObject.SetActive(true);
            panelEffectIce.SetActive(true);
        }
        isInitialized = true;
    }

    private void ActivateCurrentLevel()
    {
        enemyController[currentPoint].ActDeactEnemy(true);
        enemyController[currentPoint].ActivateAllEnemies();
        isCurrentLevelActivated = true;
    }

    public IEnumerator delayed()
    {
        yield return new WaitForSeconds(1);
        if(isOne)
            NextPoint();
    }

    public void NextPoint()
    {
        // Сбрасываем флаг активации для нового уровня
        isCurrentLevelActivated = false;

        currentPoint++;
        if (currentPoint != splines.Length)
        {
            cameraMove.spline = splines[currentPoint];
        }
        cameraMove.NormalizedT = 0;

        // Деактивируем предыдущую группу врагов
        if (currentPoint > 0 && !isSpawnPointLevel)
        {
            enemyController[currentPoint - 1].ActDeactEnemy(false);
        }
        isOne = false;
    }

    public void NextSpawnPoint()
    {
        spanwManagers[currentPoint].gameObject.SetActive(false);
        currentPoint++;
        cameraMove.spline = splines[currentPoint];
        cameraMove.NormalizedT = 0;
    }

    public IEnumerator delayNextPoint()
    {
        yield return new WaitForSeconds(1);
        if (!isSpawnPointLevel)
            NextPoint();
        else
            ActivateCurrentLevel();
    }
    
    public void Buttons(int index)
    {
        if(index == 0)
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
            currentPoint = 0;
            cameraMove.NormalizedT = 0;
            comboCrit = 0;
            score = 0;
        }
        if(index == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
            currentPoint = 0;
            cameraMove.NormalizedT = 0;
            comboCrit = 0;
            score = 0;
        }
        if (index == 2)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1;
            currentPoint = 0;
            cameraMove.NormalizedT = 0;
            comboCrit = 0;
            score = 0;
        }
    }
}