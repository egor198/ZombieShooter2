using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArtlessUI.Ui.SubPanels
{
    public class LoadSceneBtn : SubPanel
    {
        [SerializeField] private Button btn;
        [SerializeField] private string sceneName;

        public override void OnActivation()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SceneManager.LoadScene(sceneName);
        }

        public override void OnDeinitialization() 
        {
            btn.onClick.RemoveListener(OnClick);
        }
    }
}