using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArtlessUI
{
    public class VictoryPanel : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string nextScene;
        [SerializeField] private string quitScene;

        [Header("UI")]
        [SerializeField] private Button nextBtn;
        [SerializeField] private Button quitBtn;

        private void OnEnable()
        {
            Time.timeScale = 0f;
            nextBtn.onClick.AddListener(NextClick);
            quitBtn.onClick.AddListener(QuitClick);
        }

        private void QuitClick()
        {
            SceneManager.LoadScene(quitScene);
        }

        private void NextClick()
        {
            SceneManager.LoadScene(nextScene);
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            nextBtn.onClick.RemoveListener(NextClick);
            quitBtn.onClick.RemoveListener(QuitClick);
        }
    }
}