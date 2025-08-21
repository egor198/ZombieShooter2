using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArtlessUI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private Button restartBtn;
        [SerializeField] private Button quitBtn;

        [SerializeField] private string menuScene;

        private void OnEnable()
        {
            Time.timeScale = 0f;
            restartBtn.onClick.AddListener(RestartHandle);
            quitBtn.onClick.AddListener(QuitHandle);
        }

        private void QuitHandle()
        {
            SceneManager.LoadScene(menuScene);
        }

        private void RestartHandle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            restartBtn.onClick.RemoveListener(RestartHandle);
            quitBtn.onClick.RemoveListener(QuitHandle);
        }
    }
}