using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using YG;

public class ManagerNewUi : MonoBehaviour
{
    [SerializeField] private Image leaderboardButton;
    [SerializeField] private Sprite[] leaderboardButtonSprite;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private GameObject panelLeaderboard;
    [SerializeField] private GameObject panelAdsCoin;
    [SerializeField] private RectTransform rect;

    [SerializeField] private GameObject starterPackPanel;

    private void Start()
    {
        starterPackPanel.SetActive(false);
    }

    private void Update()
    {
        if(YandexGame.lang == "ru")
            textScore.text = $"Ваш опыт: {PlayerPrefs.GetInt("score")}";
        else
            textScore.text = $"Your experience: {PlayerPrefs.GetInt("score")}";
        if (LevelController.MaxReachedLevelIndex >= 2)
            leaderboardButton.sprite = leaderboardButtonSprite[1];
        else
            leaderboardButton.sprite = leaderboardButtonSprite[0];

        if(LevelController.MaxReachedLevelIndex == 3 && PlayerPrefs.GetInt("isOne", 0) == 0)
        {
            starterPackPanel.SetActive(true);
            PlayerPrefs.SetInt("isOne", 1);
        }
    }

    public void Buttons(int index)
    {
        if (index == 0 && LevelController.MaxReachedLevelIndex >= 2)
        {
            panelLeaderboard.SetActive(!panelLeaderboard.activeInHierarchy);
        }
        if (index == 1)
        {
            panelAdsCoin.SetActive(!panelAdsCoin.activeInHierarchy);
        }
        if (index == 2)
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 100);
            YandexGame.NewLeaderboardScores("test", PlayerPrefs.GetInt("score"));
        }
        if (index == 3)
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") - 100);
            YandexGame.NewLeaderboardScores("test", PlayerPrefs.GetInt("score"));
        }
        if (index == 4)
        {
            YandexGame.RewVideoShow(1);
        }
        if(index == 5)
        {
            starterPackPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    void Rewarded(int id)
    {
        if (id == 1)
            CurrenciesController.Add(CurrencyType.Coins, 50);
    }
}
