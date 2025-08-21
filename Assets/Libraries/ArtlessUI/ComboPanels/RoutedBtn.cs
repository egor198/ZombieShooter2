using UnityEngine;
using UnityEngine.UI;

namespace ArtlessUI.Ui.SubPanels
{
    public class RoutedBtn : SubPanel
    {
        [SerializeField] private Button btn;
        [SerializeField] private Router router;

        public override void OnActivation()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            navigation.InvokeMenu(router);
        }

        public override void OnInactivation()
        {
            btn.onClick.RemoveListener(OnClick);
        }
    }
}