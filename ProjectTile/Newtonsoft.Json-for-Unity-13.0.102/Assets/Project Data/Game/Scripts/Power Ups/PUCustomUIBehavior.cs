using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Watermelon
{
    public class PUCustomUIBehavior : PUUIBehavior
    {
        [Group("Refs")]
        [SerializeField] Image iconImage;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Image iconLock;
        [SerializeField] GameObject adsP;

        [Space]
        [SerializeField] SimpleBounce bounce;

        private PUCustomSettings customSettings;
        private Button button;
        private int lastLevel = -1;
        private bool wasUnlocked; // Добавлено: отслеживание предыдущего состояния

        private float timer = 10;
        public bool isAds;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClicked());
        }

        private void Start()
        {
            // Инициализируем предыдущее состояние
            //wasUnlocked = LevelController.MaxReachedLevelIndex >= customSettings.MinRequiredLevel;
            ApplyVisuals();

            StartCoroutine(CheckForUpdates());
            if (customSettings.MinRequiredLevel != 3)
                adsP = null;
        }

        private void Update()
        {
            if(adsP != null && LevelController.MaxReachedLevelIndex >= customSettings.MinRequiredLevel && settings.Save.Amount <= 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    isAds = true;
                    adsP.SetActive(true);
                    //timer = 10;
                }
                if (Input.GetMouseButton(0) && !isAds)
                {
                    timer = 10;
                    isAds = false;
                    adsP.SetActive(false);
                }   
            }
        }

        public void OnButtonClicked()
        {
            if (settings.Save.Amount > 0)
            {
                if (!behavior.IsBusy)
                {
                    if(adsP != null)
                    {
                        timer = 10;
                        isAds = false;
                        adsP.SetActive(false);
                    }
                    if (PUController.UsePowerUp(settings.Type))
                    {
                        AudioController.PlaySound(AudioController.Sounds.buttonSound);
                        bounce.Bounce();
                    }
                }
            }
            else if(settings.Save.Amount <= 0 && !isAds)
            {
                AudioController.PlaySound(AudioController.Sounds.buttonSound);

                PUController.PowerUpsUIController.PowerUpPurchasePanel.Show(settings);
            }
        }

        public void ButtonAds()
        {
            if (isAds)
            {
                if (!behavior.IsBusy)
                {
                    Reward();
                }
            }
        }

        private IEnumerator CheckForUpdates()
        {
            while (true)
            {
                if (lastLevel != LevelController.MaxReachedLevelIndex)
                {
                    ApplyVisuals();
                    lastLevel = LevelController.MaxReachedLevelIndex;
                }
                yield return new WaitForSeconds(0.3f);
            }
        }

        public void ApplyVisuals()
        {
            if (settings == null) return;

            customSettings = settings as PUCustomSettings;
            if (customSettings == null) return;

            bool isUnlocked = LevelController.MaxReachedLevelIndex >= customSettings.MinRequiredLevel;

            // Всегда обновляем визуальные элементы
            UpdateVisuals(isUnlocked);

            // Обрабатываем изменение состояния разблокировки
            if (isUnlocked != wasUnlocked)
            {
                if (isUnlocked)
                {
                    // Активируем с анимацией при разблокировке
                    gameObject.SetActive(true);
                    Activate();
                }
                else
                {
                    //gameObject.SetActive(false);
                }
                wasUnlocked = isUnlocked;
            }
            else
            {
                // Просто обновляем активность без анимации
                //gameObject.SetActive(isUnlocked);
            }

            Debug.Log($"Power-up '{customSettings.name}': " +
                      $"Unlocked={isUnlocked} " +
                      $"CurrentLevel={LevelController.MaxReachedLevelIndex} " +
                      $"RequiredLevel={customSettings.MinRequiredLevel}");
        }

        // Вынесено: обновление визуальных элементов
        private void UpdateVisuals(bool isUnlocked)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isUnlocked ? 1f : 0.4f;
            }

            if (iconImage != null)
            {
                iconImage.color = isUnlocked ? Color.white : new Color(1, 1, 1, 0.5f);
            }

            if (button != null)
            {
                button.interactable = isUnlocked;
            }
            if(iconLock != null)
                iconLock.gameObject.SetActive(!isUnlocked);
        }

        public void Reward()
        {
            YandexGame.RewVideoShow(3);
        }

        private void OnEnable()
        {
            YandexGame.RewardVideoEvent += Rewarded;
        }

        private void OnDisable()
        {
            YandexGame.RewardVideoEvent -= Rewarded;
        }

        void Rewarded(int id)
        {
            if (id == 3)
            {
                if(adsP != null)
                {
                    if (PUController.UsePowerUp(settings.Type))

                    settings.Save.Amount += 1;
                    AudioController.PlaySound(AudioController.Sounds.buttonSound);

                    bounce.Bounce();
                    isAds = false;
                    adsP.SetActive(false);
                    timer = 10;
                }
            }
        }
    }
}
