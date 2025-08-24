using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using YG;

public class SaverManagerMy : MonoBehaviour
{
    public static SaverManagerMy Instance;

    private const int SAVE_DELAY = 30;
    /*[SerializeField] InputField integerText;
    [SerializeField] InputField stringifyText;
    [SerializeField] Text systemSavesText;
    [SerializeField] Toggle[] booleanArrayToggle;*/

    private static GlobalSave globalSave;

    private void OnEnable() => YandexGame.GetDataEvent += GetLoad;
    private void OnDisable() => YandexGame.GetDataEvent -= GetLoad;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(AutoSaveCoroutine());
        //if (YandexGame.SDKEnabled)
        GetLoad();
    }

    public static T GetSaveObject<T>(int hash) where T : ISaveObject, new()
    {
        /*if (!isSaveLoaded)
        {
            Debug.LogError("Save controller has not been initialized");
            return default;
        }*/

        return globalSave.GetSaveObject<T>(hash);
    }

    public static T GetSaveObject<T>(string uniqueName) where T : ISaveObject, new()
    {
        return GetSaveObject<T>(uniqueName.GetHashCode());
    }

    public void Save()
    {
        /*YandexGame.savesData.money = int.Parse(integerText.text);
        YandexGame.savesData.newPlayerName = stringifyText.text.ToString();

        for (int i = 0; i < booleanArrayToggle.Length; i++)
            YandexGame.savesData.openLevels[i] = booleanArrayToggle[i].isOn;*/

        globalSave.Flush();

        SavesYG.SetString("Save", JsonUtility.ToJson(globalSave));
        //YandexGame.savesData.globalSave = globalSave;

        YandexGame.savesData.Save();

        YandexGame.SaveProgress();
    }

    private static IEnumerator AutoSaveCoroutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(SAVE_DELAY);

        while (true)
        {
            yield return waitForSeconds;

            Debug.Log("Save!!!");
            SaverManagerMy.Instance.Save();
        }
    }

    public void Load() => YandexGame.LoadProgress();

    public static void SaveCustom(GlobalSave globalSave)
    {
        if (globalSave != null)
        {
            globalSave.Flush();

            SavesYG.SetString("Save", JsonUtility.ToJson(globalSave));
        }
    }

    public static GlobalSave GetGlobalSave()
    {
        /*GlobalSave tempGlobalSave = BaseSaveWrapper.ActiveWrapper.Load(SAVE_FILE_NAME);

        tempGlobalSave.Init(Time.time);*/

        return globalSave;
    }

    public void GetLoad()
    {
        if (SavesYG.HasKey("Save"))
        {
            string save = SavesYG.GetString("Save");
            globalSave = JsonUtility.FromJson<GlobalSave>(save);
            globalSave.Init(Time.time);
        }
        else
        {
            globalSave = new GlobalSave();
            globalSave.Init(Time.time);
        }



        /*globalSave = YandexGame.savesData.globalSave;
        float overrideTime = -1f;
        float time;
        if (overrideTime != -1f)
        {
            time = overrideTime;
        }
        else
        {
            time = Time.time;
        }

        globalSave.Init(time);*/
        /*integerText.text = string.Empty;
        stringifyText.text = string.Empty;

        integerText.placeholder.GetComponent<Text>().text = YandexGame.savesData.money.ToString();
        stringifyText.placeholder.GetComponent<Text>().text = YandexGame.savesData.newPlayerName;

        for (int i = 0; i < booleanArrayToggle.Length; i++)
            booleanArrayToggle[i].isOn = YandexGame.savesData.openLevels[i];

        systemSavesText.text = $"Language - {YandexGame.savesData.language}\n" +
        $"First Session - {YandexGame.savesData.isFirstSession}\n" +
        $"Prompt Done - {YandexGame.savesData.promptDone}\n";*/

    }
}