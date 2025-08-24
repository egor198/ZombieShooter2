using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class PriceText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Text text2;
    public TextMeshProUGUI textDuplicate;

    private void Update()
    {
        if (text != null)
            textDuplicate.text = text.text;
        else
            textDuplicate.text = text2.text;
    }
}
