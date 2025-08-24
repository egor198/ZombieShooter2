using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationProvider : MonoBehaviour
{
    [SerializeField] string keyLang;

    private void Start()
    {
        var text = GetComponent<Text>();
        if (text != null)
        {
            text.text = Localization.Translations[keyLang];
            return;
        }

        var tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
            tmpText.text = Localization.Translations[keyLang];
    }
}
