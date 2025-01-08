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

        public void LoadScene(string sceneName, bool showLoadingScene = false, float fakeLoadingTime = 3f)
        {
            if (showLoadingScene)
            {
                gameManager.StartCoroutine(LoadSceneWithLoadingScreen(sceneName, fakeLoadingTime));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        public void LoadScene(int sceneIndex, bool showLoadingScene = false, float fakeLoadingTime = 0f)
        {
            if (showLoadingScene)
            {
                gameManager.StartCoroutine(LoadSceneWithLoadingScreen(sceneIndex, fakeLoadingTime));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            }
        }

        private IEnumerator LoadSceneWithLoadingScreen(string sceneName, float fakeLoadingTime)
        {
            // Load the loading scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");

            // Wait for one frame to ensure the loading scene is fully loaded
            yield return null;

            // Start loading the target scene
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            float elapsedTime = 0f;
            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f && elapsedTime >= fakeLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator LoadSceneWithLoadingScreen(int sceneIndex, float fakeLoadingTime)
        {
            // Load the loading scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");

            // Wait for one frame to ensure the loading scene is fully loaded
            yield return null;

            // Start loading the target scene
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            float elapsedTime = 0f;
            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f && elapsedTime >= fakeLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}