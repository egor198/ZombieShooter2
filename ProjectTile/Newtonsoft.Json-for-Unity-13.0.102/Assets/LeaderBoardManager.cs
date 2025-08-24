using UnityEngine;
using UnityEngine.UI;
using YG;

public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager Instance;

    [SerializeField]
    private LeaderboardYG leaderboardYG;

    public void Awake()
    {
        if (LeaderBoardManager.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PauseSound()
    {
        AudioListener.volume = 0f;
        Time.timeScale = 0f;
    }

    public void ResumeSound()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
    }

    public void Start()
    {
        //YandexGame.ConsumePurchases();
    }

    /*public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            PauseMusic();
        if (Input.GetKeyDown(KeyCode.P))
            ResumeMusic();
    }*/

    public void NewScore(int score)
    {
        if (SavesYG.GetInt("LeaderBoardScoreMy") < score)
        {
            SavesYG.SetInt("LeaderBoardScoreMy", score);
            YandexGame.NewLeaderboardScores(leaderboardYG.nameLB, score);
        }
    }

    
    public void PauseMusic()
    {
        
    }

    public void ResumeMusic()
    {
        
    }
}