using System.Collections.Generic;
using YG;

public static class Localization
{
    public static Dictionary<string, string> Translations;

    public static void InitTranslations()
    {
        if (YandexGame.EnvironmentData.language == "en")
        {
            Translations = new()
            {
                { "Level", "Level" },
                { "NEXT", "NEXT" },
                { "PLAY", "PLAY" },
                { "SETTINGS", "SETTINGS" },
                { "CLAIM", "CLAIM" },
                { "LOADING...", "LOADING..." },
                { "CONTINUE", "CONTINUE" },
            };
        }
        else if (YandexGame.EnvironmentData.language == "tr")
        {
            Translations = new()
            {
                { "Level", "Seviye" },
                { "NEXT", "SONRAKİ" },
                { "PLAY", "OYNA" },
                { "SETTINGS", "Ayarlar" },
                { "Sound", "Ses" },
                { "Music", "Müzik" },
                { "Push", "İTMEK" },
            };
        }
        else
        {
            Translations = new()
            {
                { "Level", "Уровень" },
                { "NEXT", "ДАЛЬШЕ" },
                { "PLAY", "ИГРАТЬ" },
                { "SETTINGS", "НАСТРОЙКИ" },
                { "CLAIM", "ЗАБРАТЬ" },
                { "LOADING...", "ЗАГРУЗКА..." },
                { "CONTINUE", "ПРОДОЛЖИТЬ" },
            };
        }
    }
}
