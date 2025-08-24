using I2.Loc;
using UnityEngine;

namespace Watermelon
{
    public abstract class PUCustomSettings : PUSettings
    {
        [Group("Variables")]
        [SerializeField] string title;
        [SerializeField] string titleRu;

        [Space]
        [SerializeField] int minRequiredLevel = 0; // ����� ����

        public string Title
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "English")
                    return title;
                else if (LocalizationManager.CurrentLanguage == "Russian")
                    return titleRu;
                else
                    return title;
            }
        }

        public int MinRequiredLevel => minRequiredLevel; // ����� ��������

        public virtual bool IsAvailable()
        {
            return LevelController.MaxReachedLevelIndex >= minRequiredLevel;
        }
    }
}
