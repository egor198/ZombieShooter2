using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager: MonoBehaviour
{
    public Toggle toggleHard;
    public TextMeshProUGUI textHp;

    private void Start()
    {
        PlayerPrefs.GetInt("Dif", 0);
        if(PlayerPrefs.GetInt("Hp") <= 1000)
            PlayerPrefs.SetInt("Hp", 1000);
        if (PlayerPrefs.GetInt("Dif") == 0)
            toggleHard.isOn = false;
        else
            toggleHard.isOn = true;
    }

    private void Update()
    {
        textHp.text = PlayerPrefs.GetInt("Hp").ToString();
        if (toggleHard.isOn)
            PlayerPrefs.SetInt("Dif", 1);
        else
            PlayerPrefs.SetInt("Dif", 0);
    }

    public void PlayButton(int indexLevel)
    {
        SceneManager.LoadScene(indexLevel);
    }

    public void Buttons(int index)
    {
        if (index == 0 && PlayerPrefs.GetInt("Hp") < 10000)
            PlayerPrefs.SetInt("Hp", PlayerPrefs.GetInt("Hp") + 1000);
        if (index == 1 && PlayerPrefs.GetInt("Hp") > 1000)
            PlayerPrefs.SetInt("Hp", PlayerPrefs.GetInt("Hp") - 1000);
    }
}
