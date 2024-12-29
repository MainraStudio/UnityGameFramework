using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MainraFramework
{
    public class SceneManager
    {
        private GameManager gameManager;

        public SceneManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void LoadScene(string sceneName, bool showLoadingUI = false)
        {
            if (showLoadingUI)
            {
                gameManager.StartCoroutine(LoadSceneAsync(sceneName));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        public void LoadScene(int sceneIndex, bool showLoadingUI = false)
        {
            if (showLoadingUI)
            {
                gameManager.StartCoroutine(LoadSceneAsync(sceneIndex));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            }
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            UIManager.Instance.ShowLoadingUI();
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                UIManager.Instance.UpdateLoadingProgress(progress);
                yield return null;
            }

            UIManager.Instance.HideLoadingUI();
        }

        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            UIManager.Instance.ShowLoadingUI();
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                UIManager.Instance.UpdateLoadingProgress(progress);
                yield return null;
            }

            UIManager.Instance.HideLoadingUI();
        }
    }
}