using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class FonsManager: MonoBehaviour
{
    public Image fon;
    public Sprite[] fonsSprite;

    void Start()
    {
        if (YandexGame.EnvironmentData.deviceType == "mobile")
            fon.sprite = fonsSprite[0];
        else
            fon.sprite = fonsSprite[1];
    }
}
