using UnityEngine;
using UnityEngine.UI;

namespace ArtlessUI.Ui.SubPanels
{
    public class ReturnedBtn : SubPanel
    {
        [SerializeField] private Button btn;

        public override void OnActivation()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            navigation.ReturnMenu();
        }

        public override void OnInactivation()
        {
            btn.onClick.RemoveListener(OnClick);
        }
    }
}