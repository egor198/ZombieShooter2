using UnityEngine;
using UnityEngine.Events;
using Watermelon.IAPStore;
using Watermelon;

namespace YG.Example
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#10e7dfffefdc42ec93b39be0c78e77cb")]
    public class ReceivingPurchaseExample : MonoBehaviour
    {
        public static ReceivingPurchaseExample Instance;

        [SerializeField] private StartedPackOffer StartedPackOffer;
        [SerializeField] private StartedPackOffer StartedPackOffer2;
        [SerializeField] private PowerUpsOffer PowerUpsOffer;
        [SerializeField] private CurrencyOffer CurrencyOffer1;
        [SerializeField] private CurrencyOffer CurrencyOffer2;
        [SerializeField] private CurrencyOffer CurrencyOffer3;
        [SerializeField] private CurrencyOffer CurrencyOffer4;
        [SerializeField] private CurrencyOffer CurrencyOffer5;
        [SerializeField] private CurrencyOffer CurrencyOffer6;

        [SerializeField] UnityEvent successPurchased;
        [SerializeField] UnityEvent failedPurchased;

        private void OnEnable()
        {
            YandexGame.PurchaseSuccessEvent += SuccessPurchased;
            YandexGame.PurchaseFailedEvent += FailedPurchased;
        }

        private void OnDisable()
        {
            YandexGame.PurchaseSuccessEvent -= SuccessPurchased;
            YandexGame.PurchaseFailedEvent -= FailedPurchased;
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //SavesYG.DeleteAll();
            //YandexGame.ResetSaveProgress();
        }

        //private void Start()
        //{
        //SoundManager.SetVolumeMusic(0f);
        //MonoSingleton<PlayerDataManager>.Instance.IsOnSoundBGM = false;
        //MonoSingleton<PlayerDataManager>.Instance.SaveOptionSound();
        //YandexGame.ConsumePurchases();
        //}

        void SuccessPurchased(string id)
        {
            successPurchased?.Invoke();

            Debug.Log("GetBuyMy = " + id);
            switch (id)
            {
                case "starterpack":
                    if (StartedPackOffer2.gameObject.activeInHierarchy)
                        StartedPackOffer2.ApplyOffer();
                    else
                        StartedPackOffer.ApplyOffer();
                    break;
                case "powerpack":
                    PowerUpsOffer.ApplyOffer();
                    break;
                case "coins1":
                    CurrencyOffer1.ApplyOffer();
                    break;
                case "coins2":
                    CurrencyOffer2.ApplyOffer();
                    break;
                case "coins3":
                    CurrencyOffer3.ApplyOffer();
                    break;
                case "coins4":
                    CurrencyOffer4.ApplyOffer();
                    break;
                case "coins5":
                    CurrencyOffer5.ApplyOffer();
                    break;
                case "coins6":
                    CurrencyOffer6.ApplyOffer();
                    break;

            }


            SaveController.MarkAsSaveIsRequired();
            SaverManagerMy.Instance.Save();

            YandexGame.SaveProgress();
            // Ваш код для обработки покупки. Например:
            //if (id == "50")
            //    YandexGame.savesData.money += 50;
            //else if (id == "250")
            //    YandexGame.savesData.money += 250;
            //else if (id == "1500")
            //    YandexGame.savesData.money += 1500;
            //YandexGame.SaveProgress();
        }

        void FailedPurchased(string id)
        {
            failedPurchased?.Invoke();
        }
    }
}