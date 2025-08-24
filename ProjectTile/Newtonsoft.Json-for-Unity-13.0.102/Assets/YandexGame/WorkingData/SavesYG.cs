using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;
        // "Технические сохранения" для работы плагина (Не удалять)

        public int HighScore;
        public string json;
        public static Dictionary<string, object> local_dictionary;

        public static SavesYG Instance;

        public SavesYG()
        {
            ResetSave();
        }

        public void ResetSave()
        {
            json = string.Empty;
            HighScore = 0;
        }

        public void SetNewHighscore()
        {
            HighScore++;
            YandexGame.NewLeaderboardScores("Leaderboard", HighScore);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return local_dictionary.GetInt(key, defaultValue);
        }

        public static void SetInt(string key, int writeValue, bool important = true)
        {
            if (local_dictionary.Set(key, writeValue))
                YandexGame.savesData.Save();
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return local_dictionary.GetBool(key, defaultValue);
        }

        public static void SetBool(string key, bool writeValue, bool important = true)
        {
            if (local_dictionary.Set(key, writeValue))
                YandexGame.savesData.Save();
        }

        public static float GetFloat(string key, float defaultValue = 0.0F)
        {
            return local_dictionary.GetFloat(key, defaultValue);
        }
        public static void SetFloat(string key, float writeValue, bool important = true)
        {
            if (local_dictionary.Set(key, writeValue))
                YandexGame.savesData.Save();
        }

        public static string GetString(string key, string defaultValue = "")
        {
            if (local_dictionary.ContainsKey(key))
            {
                return DecodeBase64(local_dictionary.GetString(key));
            }
            return defaultValue;
        }
        public static void SetString(string key, string writeValue, bool important = true)
        {
            string base64 = EncodeBase64(writeValue);
            if (local_dictionary.Set(key, base64))
                YandexGame.savesData.Save();
        }

        public static bool HasKey(string key)
        {
            return local_dictionary.ContainsKey(key);
        }
        public static void DeleteKey(string key)
        {
            if (local_dictionary.ContainsKey(key))
                local_dictionary.Remove(key);

            YandexGame.savesData.Save();
            Debug.LogError($"SaveYG: value <{key}> ERASED.");
        }

        public static void DeleteAll()
        {
            local_dictionary.Clear();
            YandexGame.savesData.Save();
            Debug.LogError("SaveYG: Player progress is ERASED.");
        }

        public void LoadSave()
        {
            local_dictionary = DecodeJSON(json);
        }

        public void Save()
        {
            json = EncodeJSON(local_dictionary);
            YandexGame.SaveProgress();
        }

        private Dictionary<string, object> DecodeJSON(string json_string)
        {
            if (!IsValidJSON(json_string)) return new();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json_string);
        }
        private string EncodeJSON(Dictionary<string, object> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }

        private bool IsValidJSON(string json_string)
        {
            if (string.IsNullOrEmpty(json_string))
                return false;

            try
            {
                JsonConvert.DeserializeObject<Dictionary<string, object>>(json_string);
            }

            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return false;
            }

            return true;
        }

        static string DecodeBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        static string EncodeBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("Input text is EMPTY.");
                return string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

    }

    public static class Extenstions
    {
        const float proximityTolerance = 1e-6f;

        public static bool IsApproximateTo(this float valueA, float valueB)
        {
            return Math.Abs(valueA - valueB) <= proximityTolerance;
        }

        public static bool GetBool(this Dictionary<string, object> dictionary, string key, bool defaultValue = false)
        {
            return Convert.ToBoolean(dictionary.GetValueOrDefault(key, defaultValue));
        }

        public static int GetInt(this Dictionary<string, object> dictionary, string key, int defaultValue = 0)
        {
            return Convert.ToInt32(dictionary.GetValueOrDefault(key, defaultValue));
        }

        public static float GetFloat(this Dictionary<string, object> dictionary, string key, float defaultValue = 0.0F)
        {
            return Convert.ToSingle(dictionary.GetValueOrDefault(key, defaultValue));
        }

        public static ulong GetUlong(this Dictionary<string, object> dictionary, string key, ulong defaultValue = 0)
        {
            return Convert.ToUInt64(dictionary.GetValueOrDefault(key, defaultValue));
        }

        public static string GetString(this Dictionary<string, object> dictionary, string key, string defaultValue = "")
        {
            return Convert.ToString(dictionary.GetValueOrDefault(key, defaultValue));
        }

        public static bool Set<TValue>(this Dictionary<string, object> dictionary, string key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            if (typeof(TValue) == typeof(float))
            {
                float valueA = Convert.ToSingle(value);
                float valueB = GetFloat(dictionary, key);
                if (valueA.IsApproximateTo(valueB))
                {
                    return false;
                }
            }

            if (dictionary[key].Equals(value))
                return false;

            dictionary[key] = value;
            return true;
        }
    }
}
