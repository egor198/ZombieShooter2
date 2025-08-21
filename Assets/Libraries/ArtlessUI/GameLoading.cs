using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtlessUI.Loading
{
    public class GameLoading : Singleton<GameLoading>
    {
        [SerializeField] private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            base.Awake();
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
        }

        public void LoadScene(string name)
        {
            StartCoroutine(StartLoadScene(name));
        }

        private IEnumerator StartLoadScene(string name)
        {
            canvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeLoadingScreen(1, 1));
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone)
                yield return null;

            yield return StartCoroutine(FadeLoadingScreen(0, 1));
            canvasGroup.gameObject.SetActive(false);
        }

        private IEnumerator FadeLoadingScreen(float targetValue, float duration)
        {
            float startValue = canvasGroup.alpha;
            float time = 0;
            while (time < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
            canvasGroup.alpha = targetValue;
        }
    }
}