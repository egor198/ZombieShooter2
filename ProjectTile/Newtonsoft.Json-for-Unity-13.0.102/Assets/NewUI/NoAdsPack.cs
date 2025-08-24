using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class NoAdsPack : MonoBehaviour
{
    public GameObject objRot;
    public InfoYG info;

    private void Start()
    {
        PlayerPrefs.GetInt("isAds", 0);
        if (PlayerPrefs.GetInt("isAds") == 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        objRot.transform.Rotate(0, 0, 0.1f);
    }

    private void OnEnable()
    {
        YandexGame.PurchaseSuccessEvent += SuccessPurchased;
    }

    private void OnDisable()
    {
        YandexGame.PurchaseSuccessEvent -= SuccessPurchased;
    }

    void SuccessPurchased(string id)
    {
        if(id == "Ads")
        {
            info.AdWhenLoadingScene = false;
            info.showFirstAd = false;
            YandexGame.StickyAdActivity(false);
            YandexGame.Instance.infoYG.fullscreenAdInterval = 10000;
            gameObject.SetActive(false);
            PlayerPrefs.SetInt("isAds", 1);
        }
    }
}
