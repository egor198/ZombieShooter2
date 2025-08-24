using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Lives Data", menuName = "Content/Data/Lives")]
    public class LivesData : ScriptableObject
    {
        public int maxLivesCount = 5;
        [Tooltip("In seconds")]public int oneLifeRestorationDuration = 1200;

        
        public string fullText
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "English")
                    return fullTextEn;
                else if (LocalizationManager.CurrentLanguage == "Russian")
                    return fullTextRu;
                else
                    return fullTextEn;
            }
        }
        public string timespanFormat
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "English")
                    return timespanFormatEn;
                else if (LocalizationManager.CurrentLanguage == "Russian")
                    return timespanFormatEn;
                else
                    return timespanFormatEn;
            }
        }
        public string longTimespanFormat
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "English")
                    return longTimespanFormatEn;
                else if (LocalizationManager.CurrentLanguage == "Russian")
                    return longTimespanFormatEn;
                else
                    return longTimespanFormatEn;
            }
        }
        [Space]
        public string fullTextEn = "FULL!";
        public string timespanFormatEn = "{0:mm\\:ss}";
        public string longTimespanFormatEn = "{0:hh\\:mm\\:ss}";

        public string fullTextRu = "ÌÀÊÑ.!";
        public string timespanFormatRu = "{0:ìì\\:ññ}";
        public string longTimespanFormatRu = "{0:÷÷\\:ìì\\:ññ}";
    }
}