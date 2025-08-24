using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class Startup : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (YandexGame.SDKEnabled == false)
            yield return null;

        Localization.InitTranslations();
        YandexGame.savesData.LoadSave();
        SceneManager.LoadSceneAsync(1);
    }
}
